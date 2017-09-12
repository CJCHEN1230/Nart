// 這是主要 DLL 檔案。

#include "stdafx.h"

#include "UseCVLibrary.h"
using namespace System::Drawing;
using namespace System::Collections::Generic;
using namespace System::Windows::Media::Media3D;

UseCVLibrary::NartPoint::NartPoint() 
{	
	ImagePoint = gcnew PointF();

	//CameraPoint = gcnew Point3D();
}

UseCVLibrary::NartPoint::NartPoint(PointF imagePoints)
{
	ImagePoint = imagePoints;

	//CameraPoint = gcnew Point3D();
}

UseCVLibrary::BWMarker::BWMarker()
{
	CornerPoint = gcnew List<NartPoint^>(4);

	NartPoint^ a = gcnew NartPoint();
	NartPoint^ b = gcnew NartPoint();
	NartPoint^ c = gcnew NartPoint();

	CornerPoint->Add(a);
	CornerPoint->Add(b);
	CornerPoint->Add(c);

}

void UseCVLibrary::BWMarker::sort()
{
	
}

int UseCVLibrary::BWMarker::CompareTo(BWMarker^ other)
{
	
	if (this->AvgRectifyY - other->AvgRectifyY > 2)
	{
		return -1;
	}
	else if (this->AvgRectifyY - other->AvgRectifyY < -2)
	{
		return 1;
	}
	else
	{
		double diff2 = this->AvgX - other->AvgX;
		if (diff2 > 0)
			return 1;
		else
			return -1;
	}


	/*if (this->AvgRectifyY - other->AvgRectifyY > 0)
		return 1;
	else
		return -1;*/
	
}

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
	for (int i = 0; i < OutputCornerPoint.size(); i++)
	{
		BWMarker^ marker = gcnew BWMarker();

		for (unsigned int j = 0; j < OutputCornerPoint.at(i).size(); j++)
		{
			marker->CornerPoint[j]->ImagePoint->X = OutputCornerPoint.at(i).at(j).x;
			marker->CornerPoint[j]->ImagePoint->Y = height - OutputCornerPoint.at(i).at(j).y;
		}

		AllMarker->Add(marker);
	}

	OutputCornerPoint.clear();

	return AllMarker;
}

