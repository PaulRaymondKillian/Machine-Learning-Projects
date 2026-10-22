// TestApp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "Matrix.h"


int _tmain(int argc, _TCHAR* argv[])
{
	Matrix *m;
	m = new Matrix(10,10);
				 m->set(0,0,-100);
				 m = m->Copy();
				 //m = 
				Matrix::Transpose();
	return 0;
}

