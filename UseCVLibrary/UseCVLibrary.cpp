// 這是主要 DLL 檔案。

#include "stdafx.h"

#include "UseCVLibrary.h"
using namespace System::Drawing;
using namespace System::Collections::Generic;
using namespace System::Windows::Media::Media3D;



UseCVLibrary::CornerPointFilter::CornerPointFilter(int number) : CameraNumber(number)
{	
	AllMarker = gcnew List<BWMarker^>();
}

List<UseCVLibrary::BWMarker^>^ UseCVLibrary::CornerPointFilter::GetCornerPoint(int width, int height, System::Byte* imageHeadPointer)
{
	std::vector<std::vector<MyPoint>> OutputCornerPoint; //最後Corner Point的容器
	pin_ptr<System::Byte> p1 = imageHeadPointer;

	unsigned char* pby1 = p1;

	CalcPoint(pby1, height, width, OutputCornerPoint);
	
	AllMarker->Clear();
	for (unsigned int i = 0; i < OutputCornerPoint.size(); i++)
	{		
		BWMarker^ marker = gcnew BWMarker();

		for (unsigned int j = 0; j < OutputCornerPoint.at(i).size(); j++)
		{
			
			Point3D^ A = gcnew Point3D(OutputCornerPoint.at(i).at(j).x, height - OutputCornerPoint.at(i).at(j).y, -2000.0); //-2000代表尚未初始化
			marker->CornerPoint->Add(*A);
			//Console::WriteLine("MARKER COUNT:" + marker->CornerPoint->Count);
		}

		AllMarker->Add(marker);
	}

	
	OutputCornerPoint.clear();
	return AllMarker;
}

UseCVLibrary::BWMarker::BWMarker()
{
	CornerPoint = gcnew List<Point3D>(4);

}
//
//UseCVLibrary::BWMarker::BWMarker(List<Point3D>^ markerPoint) 
//{
//	CornerPoint = markerPoint;
//}
//
//UseCVLibrary::BWMarker::BWMarker(Point3D A, Point3D B, Point3D C)
//{
//	CornerPoint = gcnew List<Point3D>(3);
//	CornerPoint->Add(A);
//	CornerPoint->Add(B);
//	CornerPoint->Add(C);
//
//	AvgRectifyY = (A.Z + B.Z + C.Z) / 3.0;
//	AvgX = (A.X + B.X + C.X) / 3.0;
//}

int UseCVLibrary::BWMarker::CompareTo(BWMarker^ other)
{
	if (this->AvgRectifyY - other->AvgRectifyY > 2)
		return 1;
	else if (this->AvgRectifyY - other->AvgRectifyY < -2)
		return -1;
	else
	{
		double diff2 = this->AvgX - other->AvgX;
		if (diff2 > 0)
			return 1;
		else
			return -1;
	}
}