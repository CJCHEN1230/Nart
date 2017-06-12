#ifndef GETCORNERPOINT_H
#define GETCORNERPOINT_H
#include <vector>
#ifdef BUILDING_DLL
# define DLLIMPORT __declspec(dllexport)
#else
# define DLLIMPORT __declspec(dllimport)
#endif 

struct MyPoint
{
	float x = 0;
	float y = 0;
};

extern "C"
{
	DLLIMPORT void CalcPoint(unsigned char* srcPtr1, int ImageRow, int ImageCol, std::vector<MyPoint>& OutputCornerPoint);
	//DLLIMPORT void CalcPoint(unsigned char* srcPtr, int ImageRow, int ImageCol, unsigned int& PointNumber);
}

#endif // !GETCORNERPOINT_H