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

	public ref class NartPoint
	{
		public: 
			PointF^ ImagePoint;

			Point4D CameraPoint;

			double RectifyY;

			NartPoint();
			
			NartPoint(PointF imagePoints);
			
	};

	public ref class BWMarker : public IComparable<BWMarker^>
	{
	public:

		//List<Point3D>^ CornerPoint/* = gcnew List<Point3D>(3)*/;

		List<NartPoint^>^ CornerPoint;

		double AvgRectifyY = 0;

		double AvgX = 0;

		BWMarker();

		void sort(void);
		
		virtual int CompareTo(BWMarker^ other);

	};

	public ref class CornerPointFilter
	{
		// TODO:  在此加入這個類別的方法。
	private:
		int CameraNumber = -1;

		List<BWMarker^>^ AllMarker; //儲存單張照片的所有Marker

	public:
		CornerPointFilter(int CameraNumber);

		List<BWMarker^>^ GetCornerPoint(int width, int height, System::Byte* imageHeadPointer);
	};

	
}
