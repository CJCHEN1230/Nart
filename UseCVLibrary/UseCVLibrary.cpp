// 這是主要 DLL 檔案。

#include "stdafx.h"

#include "UseCVLibrary.h"
using namespace System::Drawing;
using namespace System::Collections::Generic;




UseCVLibrary::CornerPointFilter::CornerPointFilter(int number) : CameraNumber(number)
{
	OutputCorPt = gcnew List<List<PointF>^>();
	eachEllipse = gcnew List<PointF>(3);
}

List<List<PointF>^>^ UseCVLibrary::CornerPointFilter::GetCornerPoint(int width, int height, System::Byte* imageHeadPointer)
{
	std::vector<std::vector<MyPoint>> OutputCornerPoint; //最後Corner Point的容器
	pin_ptr<System::Byte> p1 = imageHeadPointer;

	unsigned char* pby1 = p1;

	CalcPoint(pby1, height, width, OutputCornerPoint);
	
	OutputCorPt->Clear();
	//Console::WriteLine("\n\n第" + CameraNumber + "台相機");
	for (unsigned int i = 0; i < OutputCornerPoint.size(); i++)
	{
		//Console::WriteLine("\n\n第" + (i + 1) + "組");
		eachEllipse->Clear();
		for (unsigned int j = 0; j < OutputCornerPoint.at(i).size(); j++)
		{			
			eachEllipse->Add(PointF(OutputCornerPoint.at(i).at(j).x, OutputCornerPoint.at(i).at(j).y));
			
			/*eachEllipse[j].X = OutputCornerPoint.at(i).at(j).x;
			eachEllipse[j].Y = OutputCornerPoint.at(i).at(j).y;*/
			//Console::WriteLine("\n(" + eachEllipse[j].X + "," + eachEllipse[j].Y + ")");
		}
		OutputCorPt->Add(eachEllipse);

	}

	
	OutputCornerPoint.clear();
	return OutputCorPt;
}
