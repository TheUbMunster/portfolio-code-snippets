#define API __declspec(dllexport)
extern "C"
{
	API void Add(double*, double*, double*);
	API double* GetDoublePtr();
	API void DeleteDoublePtr(double* toDelete);
}