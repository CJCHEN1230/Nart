// UseCVLibrary.h

#pragma once
#include "GetCornerPoint.h"
#include <vector>
using namespace System;

namespace UseCVLibrary {

	public ref class CornerPointFilter
	{
		// TODO:  �b���[�J�o�����O����k�C
	
	public:
		CornerPointFilter();
		void GetCornerPoint(int width, int height, System::Byte* imageHeadPointer);
	};
}
