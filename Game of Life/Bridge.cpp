#include "pch.h"

#include "Bridge.h"
#include "ui_GOLGUI.h"
//#include <functional>
//#include <algorithm>

using System::Text::Encoding;

bool* GetSingle() //former return type array<bool>^
{
	 //return GOL::GOL::GetSingle();
	 bool* result = new bool[width * height];
	 for (size_t i = 0; i < width * height; i++)
	 {
			result[i] = grid[i];
	 }
	 return result;
}

void Handshake(int width, int height)
{
	 //GOL::GOL::Handshake(width, height);
	 ::width = width;
	 ::height = height;
	 if (::grid != nullptr)
	 {
			delete[] ::grid;
	 }
	 if (::buffer != nullptr)
	 {
			delete[] ::buffer;
	 }
	 ::grid = new bool[width * height];
	 ::buffer = new bool[width * height];
}

void SetValue(int pos, bool value)
{
	 //GOL::GOL::SetValue(pos, value);
	 grid[pos] = value;
}

void Step()
{
	 //GOL::GOL::Step();
	 auto Calculate = [](int i, int j) -> int
	 {
			int sum = 0;
			if (i > 0) //left check
			{
				 sum += grid[TTO(i - 1, j)] ? 1 : 0;
			}
			if (i < width - 1) //right check
			{
				 sum += grid[TTO(i + 1, j)] ? 1 : 0;
			}
			if (j > 0) //down check
			{
				 sum += grid[TTO(i, j - 1)] ? 1 : 0;
			}
			if (j < height - 1) //up check
			{
				 sum += grid[TTO(i, j + 1)] ? 1 : 0;
			}
			if (i > 0 && j > 0) //bottom left check
			{
				 sum += grid[TTO(i - 1, j - 1)] ? 1 : 0;
			}
			if (i < width - 1 && j > 0) //bottom right check
			{
				 sum += grid[TTO(i + 1, j - 1)] ? 1 : 0;
			}
			if (i > 0 && j < height - 1) //top left check
			{
				 sum += grid[TTO(i - 1, j + 1)] ? 1 : 0;
			}
			if (i < width - 1 && j < height - 1) //top right check
			{
				 sum += grid[TTO(i + 1, j + 1)] ? 1 : 0;
			}
			return sum;
	 };
	 for (int i = 0; i < width; i++)
	 {
			for (int j = 0; j < height; j++)
			{
				 int temp = Calculate(i, j);
				 if (grid[TTO(i, j)])
				 {
						//this is a white cell.
						if (temp > 1 && temp < 4)
						{
							 buffer[TTO(i, j)] = true;
						}
						else
						{
							 buffer[TTO(i, j)] = false;
						}
				 }
				 else if (temp == 3)
				 {
						buffer[TTO(i, j)] = true;
				 }
				 else
				 {
						buffer[TTO(i, j)] = false;
				 }
			}
	 }
	 //copy buffer into real grid.
	 for (int i = 0; i < width; i++)
	 {
			for (int j = 0; j < height; j++)
			{
				 grid[TTO(i, j)] = buffer[TTO(i, j)];
			}
	 }
}

inline int TTO(int i, int j)
{
	 return (i * width) + j;
}

JNIEXPORT jbooleanArray JNICALL Java_ui_GOLGUI_GetSingle(JNIEnv* env, jclass c)
{
	 bool* arr = GetSingle();
	 jboolean* buff = new jboolean[width * height];
	 for (int i = 0; i < width * height; i++)
	 {
			buff[i] = arr[i];
	 }
	 delete[] arr;
	 jbooleanArray jba = env->NewBooleanArray(width * height);
	 env->SetBooleanArrayRegion(jba, 0, width * height, buff);
	 //env->DeleteLocalRef(jba); //I think this should be uncommented? NO BAD
	 //env->ReleaseBooleanArrayElements()
	 delete[] buff;
	 return jba;
}

JNIEXPORT void JNICALL Java_ui_GOLGUI_Handshake(JNIEnv* env, jclass c, jint width, jint height)
{
	 Handshake(width, height);
}

JNIEXPORT void JNICALL Java_ui_GOLGUI_SetValue(JNIEnv* env, jclass c, jint val, jboolean isWhite)
{
	 SetValue(val, isWhite);
}

JNIEXPORT void JNICALL Java_ui_GOLGUI_Step(JNIEnv* env, jclass c)
{
	 Step();
}