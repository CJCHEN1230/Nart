// 這是主要 DLL 檔案。

#include "stdafx.h"

#include "UseCVLibrary.h"

UseCVLibrary::CornerPointFilter::CornerPointFilter()
{
	//throw gcnew System::NotImplementedException();
}

void UseCVLibrary::CornerPointFilter::GetCornerPoint(System::Byte* imageHeadPointer)
{
	std::vector<MyPoint> OutputCornerPoint;
	pin_ptr<System::Byte> p1 = imageHeadPointer;

	unsigned char* pby1 = p1;

	CalcPoint(pby1, 1200, 1600, OutputCornerPoint);
	MyPoint tempPoint;

	//Console::WriteLine("\n有" + OutputCornerPoint.size() + "個");
	for (unsigned int i = 0; i < OutputCornerPoint.size(); i++)
	{
		tempPoint.x = OutputCornerPoint.at(i).x;
		tempPoint.y = OutputCornerPoint.at(i).y;
		Console::WriteLine("\n(" + tempPoint.x + "," + tempPoint.y + ")");
	}


	OutputCornerPoint.clear();
}
