/*------------------------------------------------------------------------
    Program :           Matrix Object
    Written by:         Paul Killian
    Created:            2010-03-14
    Description:        Contains Matrix Information and Operations
 -------------------------------------------------------------------------*/

//Include Files below
#include "stdafx.h"
#include "Matrix.h"

Matrix::Matrix( void )								//Default Constructor
{	const string _METHODNAME = "Matrix()";
	try
	{
		cols = 10;
		rows = 10;
		InitializeMatrix();
	}
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
	}

}//Matrix()

Matrix::Matrix(int _rows, int _cols)
{	const string _METHODNAME = "Matrix(rows,cols)";
	try
	{
		rows = _rows;
		cols = _cols;
		InitializeMatrix();
	}
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
	}
}//Matrix(rows,cols)

Matrix::~Matrix(void)								//Destructor
{

}

void Matrix::InitializeMatrix(void)
{	const string _METHODNAME = "InitializeMatrix()";
	try
	{
		Value = new double*[rows];
		for(int i=0; i<rows; i++)
		{
			double *temp;
			temp = new double[cols];
			for(int j=0; j<cols; j++)
				temp[j] = j;
			Value[i] = temp;
		}
	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
	}//catch
}//InitializeMatrix(rows,cols)

//String^ Matrix::getRow(int row)
//{	const string _METHODNAME = "getRow(row)";
//	try
//	{
//		String ^s = "[";
//		//String^ s = "";
//		for(int i=0; i <cols; i++)
//		{
//			s = s + Value[row][i];
//			if( i != cols-1)
//				s = s + ",";
//			else
//				s = s + "]";
//		}
//		return s;
//	}//try
//	catch(...)
//	{
//		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
//	}//catch
//}
//
//String^ Matrix::getMatrix(void)
//{	const string _METHODNAME = "getMatrix()";
//	try
//	{
//		String ^s = "";
//		for(int i=0; i<rows; i++)
//			s = s + getRow(i) + "\r\n";
//
//		return s;
//	}//try
//	catch(...)
//	{
//		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
//	}//catch
//}

double Matrix::get(int row, int col)
{	const string _METHODNAME = "method()";
	try
	{
		if( row < rows && col < cols && row >= 0 && col >= 0)
			return Value[row][col];
		else
			throw exception("row or col inputted is invalid!");
	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
	}//catch
}//get(row,col)

void Matrix::set(int row, int col, double value)
{	const string _METHODNAME = "set(row,col,value)";
	try
	{
		if( row < rows && col < cols && row >= 0 && col >= 0)
			Value[row][col] = value;
		else
			throw exception("row or col inputted is invalid!");
	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
	}//catch
}//set(row,col,value)

Matrix* Matrix::Copy(void)
{	const string _METHODNAME = "Copy()";
	try
	{
		Matrix *tempNew;
		tempNew = new Matrix(rows,cols);
		for(int row=0; row<rows; row++)
			for(int col=0; col<cols; col++)
				tempNew->set(row,col,Value[row][col]);
		return tempNew;
	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
	}//catch
}//Copy




void Matrix::Transpose(void)
{	//const string _METHODNAME = "Transpose(Matrix*)";
	//try
	//{
	//	Matrix *tempNew;
	//	tempNew = new Matrix(matrix->cols,matrix->rows);
	//	for( int i=0; i<tempNew->rows; i++)
	//		for( int j=0; j<tempNew->cols; j++ )
	//			tempNew->Value[i][j] = matrix->Value[j][i];

	//	return tempNew;
	//}//try
	//catch(...)
	//{
	//	cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
	//}//catch
}




//void Matrix::method(void)
//{	const string _METHODNAME = "method()";
//	try
//	{
//		
//	}//try
//	catch(...)
//	{
//		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
//	}//catch
//}//method