#include "marshal.h"
API void Add(double* result, double* a, double* b)
{
	*result = *a + *b;
}
API double* GetDoublePtr() 
{
	return new double();
}
API void DeleteDoublePtr(double* toDelete) 
{
	delete toDelete;
	return;
}