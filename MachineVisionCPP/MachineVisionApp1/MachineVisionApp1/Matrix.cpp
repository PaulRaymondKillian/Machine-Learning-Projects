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
		//throw new exception("Error - Class: Matrix, Method: " + _METHODNAME);
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
		//throw new exception("Error - Class: Matrix, Method: " + _METHODNAME.c_str());
	}
}//Matrix(rows,cols)

Matrix::Matrix(string elements)
{
const string _METHODNAME = "Matrix(rows,cols)";
	try
	{
		vector<string> rowElements;
		vector<string> currentRow;
		Tokenize(elements,rowElements,"][");
		rows = rowElements.size();
		if(rows>0)
		{
			for(int i=0; i<rows; i++)
			{
				Tokenize(rowElements[i],currentRow,",");
				if(i==0)
				{
					cols = currentRow.size();
					InitializeMatrix();
				}
				else if( cols != currentRow.size() )
					throw new exception("Error creating matrix; column size doesnt match for all rows");

				for(int j=0; j<cols; j++)
					Value[i][j] = atof(currentRow[j].c_str());

				currentRow.clear();

			}
		}
		else
			throw new exception("Error creating matrix; invalid input parameters (Rows=0)");
		
	}
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
		//throw new exception("Error - Class: Matrix, Method: " + _METHODNAME.c_str());
	}
}//Matrix(elements)

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
				temp[j] = 0;
			Value[i] = temp;
		}
	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
		//throw new exception("Error - Class: Matrix, Method: " + _METHODNAME.c_str());
	}//catch
}//InitializeMatrix(rows,cols)

String^ Matrix::getRow(int row)
{	const string _METHODNAME = "getRow(row)";
	try
	{
		String ^s = "[";
		//String^ s = "";
		for(int i=0; i <cols; i++)
		{
			s = s + Value[row][i];
			if( i != cols-1)
				s = s + ",";
			else
				s = s + "]";
		}
		return s;
	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
		//throw new exception("Error - Class: Matrix, Method: " << _METHODNAME.c_str());
	}//catch
}

String^ Matrix::getMatrix(void)
{	const string _METHODNAME = "getMatrix()";
	try
	{
		String ^s = "";
		for(int i=0; i<rows; i++)
			s = s + getRow(i) + "\r\n";

		return s;
	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
		//throw new exception("Error - Class: Matrix, Method: " << _METHODNAME.c_str());
	}//catch
}

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
		//throw new exception("Error - Class: Matrix, Method: " << _METHODNAME.c_str());
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
		//throw new exception("Error - Class: Matrix, Method: " << _METHODNAME.c_str());
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
		//throw new exception("Error - Class: Matrix, Method: " << _METHODNAME.c_str());
	}//catch
}//Copy

double Matrix::Determinant(void)
{	const string _METHODNAME = "Determinant(Matrix*)";
	try
	{
		//Dim Det As Double
		//Dim Mat()() As Double = CopyDblArray(Matrix.Matrix, Matrix.rows, Matrix.cols)
		//Dim DArray()() As Double, S As Integer
		//Dim k, k1, i, j As Integer
		//Dim save, ArrayK As Double
		//Dim M1 As String
		//Dim Rows, Cols As Integer
		double Det;
		int S, k, k1, i, j;
		double save, ArrayK;
		string M1;
		int Rows, Cols;

		//Mat = CopyDblArray(matrix->Value, matrix->rows, matrix->cols);
		//Mat = CopyDblArray(matrix->Value, matrix->rows, matrix->cols);
		//On Error GoTo Error_Handler
		//double** newArray;
		//double **Mat;
		Mat = new double*[rows];
		for(int i=0; i<rows; i++)
			Mat[i] = new double[cols];

		for(int i=0; i<rows; i++)
			for(int j=0; j<cols; j++)
				Mat[i][j] = Value[i][j];
		//Mat = new double*[rows];
		//for(int i=0; i<rows; i++)
		//{
		//	double *temp;
		//	temp = new double[cols];
		//	for(int j=0; j<cols; j++)
		//		temp[j] = i+j;
		//	Mat[i] = temp;
		//}



		//Rows = Matrix.rows
		//Cols = Matrix.cols
		Rows = rows;
		Cols = cols;

		//If Rows <> Cols Then GoTo Error_Dimension
		if( Rows != Cols)
			throw new exception("Invalid for calculating determinant, must be square matrix");

		//S = Rows - 1
		//Det = 1
		//DArray = Mat.Clone()
		S = Rows - 1;
		Det = 1;
		//DArray = CopyDblArray(Mat, Rows, Cols);
		//DArray = CopyDblArray(Value, rows, cols);
		DArray = new double*[rows];
		for(int i=0; i<rows; i++)
			DArray[i] = new double[cols];

		for(int i=0; i<rows; i++)
			for(int j=0; j<cols; j++)
				DArray[i][j] = Value[i][j];


		//For k = 0 To S
		for(int k=0; k<=S; k++)
		{
		//	If DArray(k)(k) = 0 Then
			if(DArray[k][k] == 0)
			{
		//		j = k
				j = k;
		//		Do While ((j < S) And (DArray(k)(j) = 0))
				while((j<S) && (DArray[k][j] == 0))
				{
		//			j = j + 1
					j = j+1;
		//		Loop
				}//while
		//		If DArray(k)(j) = 0 Then
				if( DArray[k][j] == 0 )
				{
		//			Det = 0
		//			Exit Function
					Det = 0;
					return Det;
				}
		//		Else
				else
				{
		//			For i = k To S
					for(int i=k; i<=S; i++)
					{
		//				save = DArray(i)(j)
		//				DArray(i)(j) = DArray(i)(k)
		//				DArray(i)(k) = save
						save = DArray[i][j];
						DArray[i][j] = DArray[i][k];
						DArray[i][k] = save;
		//			Next i
					}//for
		//		End If
				}//if

		//		Det = -Det
				Det = -Det;
		//	End If
			}//if
		//	ArrayK = DArray(k)(k)
		//	Det = Det * ArrayK
			ArrayK = DArray[k][k];
			Det = Det * ArrayK;
		//	If k < S Then
			if( k < S )
			{
		//		k1 = k + 1
				k1 = k + 1;
		//		For i = k1 To S
				for(int i=k1; i<=S; i++)
				{
		//			For j = k1 To S
					for( int j=k1; j<=S; j++)
					{
		//				DArray(i)(j) = DArray(i)(j) - DArray(i)(k) * (DArray(k)(j) / ArrayK)
						DArray[i][j] = DArray[i][j] - DArray[i][k] * (DArray[k][j] / ArrayK);
		//			Next j
					}//for
		//		Next i
				}//for
		//	End If
			}//if
		//Next
		}//for

		//Return Det
		return Det;

	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
		//throw new exception("Error - Class: Matrix, Method: " << _METHODNAME.c_str());
	}//catch
}//Determinant(Matrix)



//////////////////////////////////////////////////////////
//														//
//				Begin Static Methods					//
//														//
//////////////////////////////////////////////////////////


Matrix* Matrix::Transpose(Matrix *matrix)
{	const string _METHODNAME = "Transpose(Matrix*)";
	try
	{
		Matrix *tempNew;
		tempNew = new Matrix(matrix->cols,matrix->rows);
		for( int i=0; i<tempNew->rows; i++)
			for( int j=0; j<tempNew->cols; j++ )
				tempNew->Value[i][j] = matrix->Value[j][i];

		return tempNew;
	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
		//throw new exception("Error - Class: Matrix, Method: " << _METHODNAME.c_str());
	}//catch
}//Transpose(Matrix*)

Matrix* Matrix::Multiply(Matrix *matrix, double scaler)
{	const string _METHODNAME = "Multiply(Matrix*,double)";
	try
	{
		Matrix *tempNew;
		tempNew = new Matrix(matrix->rows,matrix->cols);
		for( int i=0; i<tempNew->rows; i++)
			for( int j=0; j<tempNew->cols; j++ )
				tempNew->Value[i][j] = matrix->Value[i][j] * scaler;

		return tempNew;
	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
		//throw new exception("Error - Class: Matrix, Method: " << _METHODNAME.c_str());
	}//catch
}//Multiply(Matrix*,scaler)

Matrix* Matrix::Multiply(Matrix *matrix1, Matrix *matrix2)
{	const string _METHODNAME = "Multiply(Matrix*,Matrix*)";
	try
	{
		if(matrix1->cols != matrix2->rows)
			throw new exception("Invalid matrices for multiplication; matrix1 cols != matrix2 rows");

		Matrix *tempNew;
		tempNew = new Matrix(matrix1->rows,matrix2->cols);
		for( int resultRow=0; resultRow<tempNew->rows; resultRow++)
			for( int resultCol=0; resultCol<tempNew->cols; resultCol++ )
				tempNew->Value[resultRow][resultCol] = matrix1->Value[resultRow][resultCol] * matrix2->Value[resultCol][resultRow];

		return tempNew;
	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
		//throw new exception("Error - Class: Matrix, Method: " << _METHODNAME.c_str());
	}//catch
}//Multiply(Matrix*,scaler)





//////////////////////////////////////////////////////////
//														//
//				Begin Helper Methods					//
//		(Methods to Assist Matrix Operations)			//
//////////////////////////////////////////////////////////

//double** Matrix::CopyDblArray(Matrix* matrix )
double** Matrix::CopyDblArray(double** Array, int _rows, int _cols )
{	const string _METHODNAME = "CopyDblArray()";
	try
	{
		//int _rows, _cols;
		//_rows = matrix->rows;
		//_cols = matrix->cols;

		//double** newArray;
		//newArray = new double*[_rows];
		//for(int i=0; i<_rows; i++)
		//{
		//	double* temp;
		//	temp = new double[_cols];
		//	for(int j=0; j<_cols; j++)
		//		temp[j] = i*j;
		//	newArray[i] = temp;
		//}
		double** newArray;
		newArray = new double*[_rows];
		for(int i=0; i<_rows; i++)
			newArray[i] = new double[_cols];

		for(int i=0; i<_rows; i++)
			for(int j=0; j<_cols; j++)
				newArray[i][j] = Array[i][j];
				//newArray[i][j] = arr[i][j];
				//newArray[i][j] = matrix->Value[i][j];
				

		return newArray;
	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
		//throw new exception("Error - Class: Matrix, Method: " << _METHODNAME.c_str());
	}//catch
}//CopyDblArray

void Matrix::Tokenize(const string& str, vector<string>& tokens, const string& delimiters)
{	const string _METHODNAME = "Tokenize()";
	try
	{
		// Skip delimiters at beginning.
		string::size_type lastPos = str.find_first_not_of(delimiters, 0);
		// Find first "non-delimiter".
		string::size_type pos     = str.find_first_of(delimiters, lastPos);

		while (string::npos != pos || string::npos != lastPos)
		{
			// Found a token, add it to the vector.
			tokens.push_back(str.substr(lastPos, pos - lastPos));
			// Skip delimiters.  Note the "not_of"
			lastPos = str.find_first_not_of(delimiters, pos);
			// Find next "non-delimiter"
			pos = str.find_first_of(delimiters, lastPos);
		}
	}//try
	catch(...)
	{
		cout << "Error - Class: Matrix, Method: " << _METHODNAME.c_str();
		//throw new exception("Error - Class: Matrix, Method: " << _METHODNAME.c_str());
	}//catch
}//Tokenize





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