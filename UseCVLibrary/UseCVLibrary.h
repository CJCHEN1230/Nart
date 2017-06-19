// UseCVLibrary.h

#pragma once
#include "GetCornerPoint.h"
#include <vector>
using namespace System;


namespace UseCVLibrary {
	using namespace System;
	using namespace System::Windows;
	using namespace System::Drawing;
	using namespace System::Collections::Generic;


	public ref class CornerPointFilter
	{
		// TODO:  �b���[�J�o�����O����k�C
	private:
		int CameraNumber = -1;
		
		List<List<PointF>^>^ OutputCorPt;
		List<PointF>^ eachEllipse;
	public:
		CornerPointFilter(int CameraNumber);
		List<List<PointF>^>^ GetCornerPoint(int width, int height, System::Byte* imageHeadPointer);
	};
}
