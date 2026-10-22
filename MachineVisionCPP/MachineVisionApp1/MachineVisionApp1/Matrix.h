/*------------------------------------------------------------------------
    Program :           Player header file
    Written by:         Paul Killian
    Created:            3-13-06
    Description:        Header for Player.cpp
 -------------------------------------------------------------------------*/

#ifndef MATRIX_H
#define MATRIX_H

//Include Files below
#include "stdafx.h"
#include <iostream>
#include <cstring>
#include <vector>


using namespace std;
using namespace System;

class Matrix
{

public:
	//Constructors
	Matrix( void );								//Default Constructor
	Matrix::Matrix(int _rows, int _cols);
	Matrix::Matrix(string elements);
	~Matrix( void );								//Destructor
	
	//Public Methods
	String^ Matrix::getRow(int row);
	String^ Matrix::getMatrix(void);
	double Matrix::get(int row, int col);				//Dont use this for mass calculations (multiplication). Access data directly for less overhead
	void Matrix::set(int row, int col, double value);	//Dont use this for mass calculations (multiplication). Access data directly for less overhead
	Matrix* Matrix::Copy(void);
	double Matrix::Determinant(void);
	//Public Static Methods
	static Matrix* Transpose(Matrix *matrix);
	static Matrix* Multiply(Matrix *matrix, double scaler);
	static Matrix* Multiply(Matrix *matrix1, Matrix *matrix2);
	
	//Public Variables
	double **Value;		//Make public to prevent requirement to use method to access values, to lessen overhead/more efficient processing
	double **Mat;		//Used to Calc Determinant
	double **DArray;
	int cols;
	int rows;


private:
	//Private Methods
	void Matrix::InitializeMatrix(void);
	double** Matrix::CopyDblArray(double**, int _rows, int _cols);
	void Matrix::Tokenize(const string& str, vector<string>& tokens, const string& delimiters);
	
	//Private Variables
	

};


#endif