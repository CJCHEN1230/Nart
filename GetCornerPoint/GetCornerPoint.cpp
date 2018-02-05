// GetCornerPoint.cpp : �w�q DLL ���ε{�����ץX�禡�C
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

DLLIMPORT void CalcPoint(unsigned char* srcPtr1, int ImageRow, int ImageCol, vector<vector<MyPoint>>& OutputCornerPoint)
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

	concurrent_vector<Point2f> answers;  //�̫ᵪ�ת��x�s�e��


	Mat threshold_output; //�G�Ȥƫᵲ�G
	adaptiveThreshold(src1, threshold_output, 255, ADAPTIVE_THRESH_MEAN_C, THRESH_BINARY, 65, 0);//�۾A�ʤG�ȤƵ��G
	blur(threshold_output, threshold_output, Size(3, 3));  //�ҽk���o�p����
	vector<vector<Point>> contours;
	/// �N�G�ȤƵ��G�M�����
	findContours(threshold_output, contours,/* hierarchy,*/CV_RETR_LIST, CV_CHAIN_APPROX_SIMPLE/*, Point(0, 0)*/);
	/// ��C�ӧ�쪺�����Ыإi�ɱת���ɮةM���	
	concurrent_vector<vector<Point>> newContours;
	concurrent_vector<RotatedRect> EllipseSet;
	//�N�Ҧ���������ƭp��ӿz��
	parallel_for(0u, (unsigned int)contours.size(), [&newContours, &EllipseSet, &contours](int i) {
		RotatedRect temp;
		if (contours.at(i).size() < 60 || contours.at(i).size() > 300)
		{
			return;
		}
		else if (CalcEllipse(temp = fitEllipse(Mat(contours.at(i))), contours.at(i)) < 5)//4���������ֿn�~�t��
		{
			newContours.push_back(contours.at(i));//�N�ŦX���󪺽����A�s�J�s��������
			EllipseSet.push_back(temp);//�N�ŦX���󪺽�����J���M���A�s�U�^�Ǫ�RotatedRect���
		}
	});

	if (newContours.empty())
	{
		return;
	}

	//�Ы�ROI mask
	Mat mask(src1.size(), CV_8UC1, Scalar(0));

	//tempNewContours�ΨӦs�W��newcontours�A�]�����A�L�k�����bdrawContours��
	vector<vector<Point>> tempNewContours(newContours.size());
	tempNewContours.assign(newContours.begin(), newContours.end());

	////�ХX����ϰ�A�N���϶��ϥ�
	for (size_t i = 0; i < tempNewContours.size(); i++) {
		drawContours(mask, tempNewContours, i, Scalar(255), CV_FILLED);
	}



	concurrent_vector<vector<MyPoint>> EachEllipsePoint;//�s�C�Ӿ�ꤺ�����I



														//������B��O���O���쪺���p�⨤�I
	parallel_for(0u,/*1u*/ (unsigned int)EllipseSet.size(), [&src1, &mask, &EllipseSet, &EachEllipsePoint](int value)
	{
		//���O����Ҧb����m�^�����̤p���

		//Rect ROI = EllipseSet.at(value).boundingRect();
		Rect ROI = getBoundingRect(EllipseSet.at(value));

		//���P�_��ɦ��S���W�L
		if (ROI.x <= 0 || ROI.y <= 0 || (ROI.x + ROI.width > src1.cols) || (ROI.y + ROI.height > src1.rows)) {
			return;
		}

		Mat srcTemp = src1(ROI);
		Mat maskTemp = mask(ROI);


		/// Shi-Tomasi���ѼƳ]�m  
		double qualityLevel = 0.001;	//�̤p�S�x�Ȥp��qualityLevel*�̤j�S�x�Ȫ��I�N�Q����
		double minDistance = 20;		//�⨤�I���̤p�Z�� 
		int blockSize = 3;
		bool useHarrisDetector = false;   //���ϥ�Harris  
		double k = 0.04;
		vector<Point2f> corners;//�O�����I�A�Φbshi_tomashi��

								// ����Shi-Tomasi���I�˴���k   
		goodFeaturesToTrack(srcTemp,
			corners,
			3,
			qualityLevel,
			minDistance,
			maskTemp,   //���ѼƬ��P����ϰ�
			blockSize,
			useHarrisDetector,
			k);
		//�P�_��ꤤ�ߦ��S���b��쪺���I����
		if (!IsInTriangle(EllipseSet.at(value), corners, ROI)) {
			return;
		}


		/// ���I��m��ǤưѼ�  
		Size winSize = Size(5, 5);
		Size zeroZone = Size(-1, -1);
		TermCriteria criteria = TermCriteria(
			CV_TERMCRIT_EPS + CV_TERMCRIT_ITER,
			40, //maxCount=40  
			0.001);  //epsilon=0.001  
		cornerSubPix(srcTemp, corners, winSize, zeroZone, criteria);// �p���Ǥƫ᪺���I��m 

																	///�o�q�e���n�e���A�[
																	//ellipse(src1, EllipseSet.at(value), Scalar(255), 1, 8);


		vector<MyPoint> myPointVec(3);	//��s��MyPoint���Ȧs��
		vector<MyPoint>::iterator it2 = myPointVec.begin();
		//�[�WROI����m�~�O��Ϧ�m
		for (vector<Point2f>::iterator it = corners.begin(); it != corners.end(); it++)
		{
			circle(src1, Point2f((*it2).x = (*it).x + ROI.x, (*it2).y = (*it).y + ROI.y), 10, Scalar(255), 2, 8, 0);//�e�X�Ҧ����I��m�A�ñN��m�s�iMyPoint2f��
																													/*			(*it2).x = (*it).x + ROI.x;
																													(*it2).y = (*it).y + ROI.y;		*/
			it2++;
		}

		EachEllipsePoint.push_back(myPointVec);

	});

	OutputCornerPoint.reserve(EachEllipsePoint.size());

	OutputCornerPoint.assign(EachEllipsePoint.begin(), EachEllipsePoint.end());





	//cout << "\n�Ĥ@�i��" << OutputCornerPoint.at(0).size() << "���I";
	//cout << "\n�ĤG�i��" << OutputCornerPoint.at(1).size() << "���I";
	//if (OutputCornerPoint.at(0).size() == OutputCornerPoint.at(1).size()) {
	//	cout << "\n��i�Ϥ@��";
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

	if (corners.size() < 3) { //�p�G��쪺���I�C��3�ӫh�ⳣ�Ǻ�
		return false;
	}
	//�H�U�T�ӬO�V�q
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

	double t = 0; //t���ѼƱN����ѼƤƫ᪺�����ܼ�

	t = atan(-1 * b * tan(phi) / a); //�ѼƤƫ�x��t�L����0�A�i�o��ꥪ�k������ɪ�x�ȡA�ۮt180��
	double x1 = ellipse.center.x + a*cos(t)*cos(phi) - b*sin(t)*sin(phi);
	t = t + PI;
	double x2 = ellipse.center.x + a*cos(t)*cos(phi) - b*sin(t)*sin(phi);
	ROI.x = x1 > x2 ? x2 : x1;


	t = t - PI / 2.0;//�ѼƤƫ�y��t�L����0�A�i�o���W�U������ɪ�y�ȡA�ۮt180��
	double y1 = ellipse.center.y + b*sin(t)*cos(phi) + a*cos(t)*sin(phi);
	t = t + PI;
	double y2 = ellipse.center.y + b*sin(t)*cos(phi) + a*cos(t)*sin(phi);
	ROI.y = y1 > y2 ? y2 : y1;

	ROI.width = fabs(x2 - x1);
	ROI.height = fabs(y2 - y1);

	return ROI;
}