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
	using namespace System::Windows::Media::Media3D;


	public ref class BWMarker : public IComparable<BWMarker^>
	{
	public:

		List<Point3D>^ CornerPoint/* = gcnew List<Point3D>(3)*/;

		double AvgRectifyY = 0;

		double AvgX = 0;

		BWMarker();

		//BWMarker(List<Point3D>^ markerPoint);

		//BWMarker(Point3D A, Point3D B, Point3D C);


		virtual int CompareTo(BWMarker^ other);

	};

	public ref class CornerPointFilter
	{
		// TODO:  �b���[�J�o�����O����k�C
	private:
		int CameraNumber = -1;

		List<BWMarker^>^ AllMarker; //�x�s��i�Ӥ����Ҧ�Marker

	public:
		CornerPointFilter(int CameraNumber);

		List<BWMarker^>^ GetCornerPoint(int width, int height, System::Byte* imageHeadPointer);
	};

	
}
