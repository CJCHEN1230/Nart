// 這是主要 DLL 檔案。

#include "stdafx.h"

#include "UseCVLibrary.h"

UseCVLibrary::CornerPointFilter::CornerPointFilter()
{
	
}

void UseCVLibrary::CornerPointFilter::GetCornerPoint(int width, int height, System::Byte* imageHeadPointer)
{
	std::vector<std::vector<MyPoint>> OutputCornerPoint;
	pin_ptr<System::Byte> p1 = imageHeadPointer;

	unsigned char* pby1 = p1;

	CalcPoint(pby1, height, width, OutputCornerPoint);
	MyPoint tempPoint;

	
	for (unsigned int i = 0; i < OutputCornerPoint.size(); i++)
	{
		Console::WriteLine("\n\n第" + (i + 1) + "組");
		for (unsigned int j = 0; j < OutputCornerPoint.at(i).size(); j++)
		{			
			Console::WriteLine("\n(" + OutputCornerPoint.at(i).at(j).x + "," + OutputCornerPoint.at(i).at(j).y + ")");
		}
	}


	OutputCornerPoint.clear();
}
