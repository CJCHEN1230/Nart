// GetCornerPoint.cpp : 定義 DLL 應用程式的匯出函式。
//

#include "stdafx.h"
#define BUILDING_DLL
#include "GetCornerPoint.h"
#include "opencv2/core/core.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/calib3d/calib3d.hpp"
//#include "opencv2/contrib/contrib.hpp"
#include "opencv2/features2d/features2d.hpp"
#include "opencv2/highgui/highgui.hpp"
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <cmath>
#include <time.h>
#include <string>
#include <ppl.h>
#include <concurrent_vector.h>

#define PI 3.14159265358979323846  
using namespace cv;
using namespace std;
using namespace concurrency;




//const unsigned int PicNumber = 2;

double CalcEllipse(RotatedRect&, vector<Point>&);
inline bool IsInTriangle(RotatedRect&, vector<Point2f>&, Rect&);
inline Rect getBoundingRect(RotatedRect& ellipse);

DLLIMPORT void CalcPoint(unsigned char* srcPtr1, int ImageRow, int ImageCol, vector<MyPoint>& OutputCornerPoint)
//DLLIMPORT void CalcPoint(unsigned char* srcPtr, int ImageRow, int ImageCol, unsigned int& PointNumber)
{
	/*Mat src1(ImageRow, ImageCol, CV_8U);
	Mat src2(ImageRow, ImageCol, CV_8U);
	Mat_<uchar>::iterator it1 = src1.begin<uchar>();
	Mat_<uchar>::iterator it2 = src2.begin<uchar>();
	Mat_<uchar>::iterator itend1 = src1.end<uchar>();
	for (; it1 != itend1;) {
	(*it1++) = *srcPtr1++;
	(*it2++) = *srcPtr2++;
	}*/

	/*unsigned char* srcPtr11 = new unsigned char[ImageRow*ImageCol];
	unsigned char* srcPtr22 = new unsigned char[ImageRow*ImageCol];

	for (size_t i = 0; i < ImageRow*ImageCol; i++)
	{
	srcPtr11[i] = srcPtr1[(ImageRow*ImageCol-i) * 3];
	}
	for (size_t i = 0; i < ImageRow*ImageCol; i++)
	{
	srcPtr22[i] = srcPtr2[(ImageRow*ImageCol-i) * 3];
	}

	*/
	Mat src1(ImageRow, ImageCol, CV_8U, srcPtr1);

	concurrent_vector<Point2f> answers;  //最後答案的儲存容器，分別為第一張跟第二張的點


	Mat threshold_output; //二值化後結果
	adaptiveThreshold(src1, threshold_output, 255, ADAPTIVE_THRESH_MEAN_C, THRESH_BINARY, 65, 0);//自適性二值化結果
	blur(threshold_output, threshold_output, Size(3, 3));  //模糊化濾小輪廓
	vector<vector<Point>> contours;
	/// 將二值化結果尋找輪廓
	findContours(threshold_output, contours,/* hierarchy,*/CV_RETR_LIST, CV_CHAIN_APPROX_SIMPLE/*, Point(0, 0)*/);
	/// 對每個找到的輪廓創建可傾斜的邊界框和橢圓	
	concurrent_vector<vector<Point>> newContours;
	concurrent_vector<RotatedRect> EllipseSet;
	//將所有輪廓平行化計算來篩選
	parallel_for(0u, (unsigned int)contours.size(), [&newContours, &EllipseSet, &contours](int i) {
		if (contours.at(i).size() < 100 || contours.at(i).size() > 500)
		{
			return;
		}
		else if (CalcEllipse(fitEllipse(Mat(contours.at(i))), contours.at(i)) < 5)//4為橢圓輪廓累積誤差值
		{
			newContours.push_back(contours.at(i));//將符合條件的輪廓再存入新的輪廓當中
			EllipseSet.push_back(fitEllipse(Mat(contours.at(i))));//將符合條件的輪廓丟入擬和橢圓，存下回傳的RotatedRect資料
		}
	});

	if (newContours.empty())
	{
		return;
	}

	//創建ROI mask
	Mat mask(src1.size(), CV_8UC1, Scalar(0));

	//tempNewContours用來存上面newcontours，因為型態無法直接轉
	vector<vector<Point>> tempNewContours;
	for (concurrent_vector<vector<Point>>::iterator it = newContours.begin(); it != newContours.end(); it++) {
		tempNewContours.push_back(*it);
	}

	////標出關鍵區域，將橢圓區塊反白
	for (size_t i = 0; i < tempNewContours.size(); i++) {
		drawContours(mask, tempNewContours, i, Scalar(255), CV_FILLED);
	}


	Mat srcCopy = src1.clone();//複製一份進去做平行運算
	concurrent_vector<Point2f> EachPicPoint;//存每張照片的點
	concurrent_vector<RotatedRect> newEllipseSet;


	//此平行運算是分別對找到的橢圓計算角點
	parallel_for(0u,/*1u*/ (unsigned int)EllipseSet.size(), [&srcCopy, &mask, &EllipseSet, &newEllipseSet, &EachPicPoint](int value)
	{
		//分別對橢圓所在的位置擷取成最小方框

		//Rect ROI = EllipseSet.at(value).boundingRect();
		Rect ROI = getBoundingRect(EllipseSet.at(value));

		//先判斷邊界有沒有超過
		if (ROI.x <= 0 || ROI.y <= 0 || (ROI.x + ROI.width > srcCopy.cols) || (ROI.y + ROI.height > srcCopy.rows)) {
			return;
		}

		Mat srcTemp = srcCopy(ROI);
		Mat maskTemp = mask(ROI);


		/// Shi-Tomasi的參數設置  
		double qualityLevel = 0.001;	//最小特徵值小於qualityLevel*最大特徵值的點將被忽略
		double minDistance = 10;		//兩角點間最小距離 
		int blockSize = 3;
		bool useHarrisDetector = false;   //不使用Harris  
		double k = 0.04;
		vector<Point2f> corners;//記錄角點，用在shi_tomashi裡

								// 應用Shi-Tomasi角點檢測算法   
		goodFeaturesToTrack(srcTemp,
			corners,
			3,
			qualityLevel,
			minDistance,
			maskTemp,   //此參數為感興趣區域
			blockSize,
			useHarrisDetector,
			k);
		//判斷橢圓中心有沒有在找到的角點內部
		if (!IsInTriangle(EllipseSet.at(value), corners, ROI)) {
			return;
		}

		newEllipseSet.push_back(EllipseSet.at(value));

		/// 角點位置精準化參數  
		Size winSize = Size(5, 5);
		Size zeroZone = Size(-1, -1);
		TermCriteria criteria = TermCriteria(
			CV_TERMCRIT_EPS + CV_TERMCRIT_ITER,
			40, //maxCount=40  
			0.001);  //epsilon=0.001  
		cornerSubPix(srcTemp, corners, winSize, zeroZone, criteria);// 計算精準化後的角點位置 

		for (vector<Point2f>::iterator it = corners.begin(); it != corners.end(); it++) {
			EachPicPoint.push_back(Point2f((*it).x + ROI.x, (*it).y + ROI.y));
		}

	});
	//將點存進answer		
	for (concurrent_vector<Point2f>::iterator it = EachPicPoint.begin(); it != EachPicPoint.end(); it++) {
		answers.push_back(*it);
	}

	for (size_t i = 0; i < EachPicPoint.size(); i++) {
		// 標示出角點  
		circle(src1, EachPicPoint.at(i), 10, Scalar(255), 2, 8, 0);
	}





	//將點轉存進corners
	//vector<vector<MyPoint>> OutputCornerPoint(2);
	MyPoint tempPoint;
	for (unsigned int i = 0; i < answers.size(); i++)
	{
		//cout << "\n\n第" << i + 1 << "張圖";		
		tempPoint.x = answers.at(i).x;
		tempPoint.y = answers.at(i).y;
		//cout << "\n(" << tempPoint.x << "," << tempPoint.y << ")";

		OutputCornerPoint.push_back(tempPoint);

	}
	//cout << "\n第一張圖" << OutputCornerPoint.at(0).size() << "個點";
	//cout << "\n第二張圖" << OutputCornerPoint.at(1).size() << "個點";
	//if (OutputCornerPoint.at(0).size() == OutputCornerPoint.at(1).size()) {
	//	cout << "\n兩張圖一樣";
	//}
	//system("pause");

	//imshow("src1", src1);
	//imshow("src2", src2);

	//waitKey(0);



}


inline double CalcEllipse(RotatedRect& ellipseData, vector<Point>& pointData)
{

	double CumulativeError = 0;
	double alpha = ellipseData.angle * 2 * PI / 360;
	double a = ellipseData.size.width / 2;
	double b = ellipseData.size.height / 2;
	double x0 = ellipseData.center.x;
	double y0 = ellipseData.center.y;

	for (size_t i = 0; i<pointData.size(); i++)
	{
		CumulativeError += abs(pow((pointData.at(i).x - x0)*cos(alpha) + (pointData.at(i).y - y0)*sin(alpha), 2) / pow(a, 2) +
			pow((pointData.at(i).x - x0)*sin(alpha) - (pointData.at(i).y - y0)*cos(alpha), 2) / pow(b, 2) - 1);
	}

	return CumulativeError;
}

inline bool IsInTriangle(RotatedRect& ellipse, vector<Point2f>& corners, Rect& ROI) {

	if (corners.size() < 3) { //如果找到的角點低於3個則算都甭算
		return false;
	}
	//以下三個是向量
	Point2f PA(corners[0].x + ROI.x - ellipse.center.x, corners[0].y + ROI.y - ellipse.center.y);
	Point2f PB(corners[1].x + ROI.x - ellipse.center.x, corners[1].y + ROI.y - ellipse.center.y);
	Point2f PC(corners[2].x + ROI.x - ellipse.center.x, corners[2].y + ROI.y - ellipse.center.y);

	float t1 = PA.x*PB.y - PA.y*PB.x;
	float t2 = PB.x*PC.y - PB.y*PC.x;
	float t3 = PC.x*PA.y - PC.y*PA.x;

	return t1*t2 >= 0 && t1*t3 >= 0;
}

inline Rect getBoundingRect(RotatedRect& ellipse) {

	Rect ROI;
	double phi = ellipse.angle * 2.0 * PI / 360;
	double a = ellipse.size.width / 2.0;
	double b = ellipse.size.height / 2.0;

	double t = 0; //t為參數將協橢圓參數化後的角度變數

	t = atan(-1 * b * tan(phi) / a); //參數化後x對t微分為0，可得橢圓左右兩邊邊界的x值，相差180度
	double x1 = ellipse.center.x + a*cos(t)*cos(phi) - b*sin(t)*sin(phi);
	t = t + PI;
	double x2 = ellipse.center.x + a*cos(t)*cos(phi) - b*sin(t)*sin(phi);
	ROI.x = x1 > x2 ? x2 : x1;


	t = t - PI / 2.0;//參數化後y對t微分為0，可得橢圓上下兩邊邊界的y值，相差180度
	double y1 = ellipse.center.y + b*sin(t)*cos(phi) + a*cos(t)*sin(phi);
	t = t + PI;
	double y2 = ellipse.center.y + b*sin(t)*cos(phi) + a*cos(t)*sin(phi);
	ROI.y = y1 > y2 ? y2 : y1;

	ROI.width = fabs(x2 - x1);
	ROI.height = fabs(y2 - y1);

	return ROI;
}