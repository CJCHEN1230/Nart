// UseCVLibrary.h

#pragma once
#include "GetCornerPoint.h"
#include <vector>
using namespace System;

namespace UseCVLibrary {

	public ref class CornerPointFilter
	{
		// TODO:  在此加入這個類別的方法。
	public:
		CornerPointFilter();
		void GetCornerPoint(System::Byte* imageHeadPointer);
	};
}
