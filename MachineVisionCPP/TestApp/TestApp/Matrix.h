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


using namespace std;
//using namespace System;

class Matrix
{

public:
	//Constructors
	Matrix( void );								//Default Constructor
	Matrix::Matrix(int _rows, int _cols);
	~Matrix( void );								//Destructor
	
	//Public Methods
	//string Matrix::getRow(int row);
	//string Matrix::getMatrix(void);
	double Matrix::get(int row, int col);				//Dont use this for mass calculations (multiplication). Access data directly for less overhead
	void Matrix::set(int row, int col, double value);	//Dont use this for mass calculations (multiplication). Access data directly for less overhead
	Matrix* Matrix::Copy(void);

	//Public Static Methods
	static void Transpose(void);
	
	//Public Variables
	double **Value;		//Make public to prevent requirement to use method to access values, to lessen overhead/more efficient processing
	int cols;
	int rows;


private:
	//Private Methods
	void Matrix::InitializeMatrix(void);
	
	//Private Variables
	

};


#endif