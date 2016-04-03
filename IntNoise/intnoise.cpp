//#include <iostream>
#include "mex.h"

double IntNoise(int x)			 
{
	x = (x<<13) ^ x;
    double res = ( 1.0 - ( (x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);

	return res;
}

void mexFunction(
		int nlhs, // Number of left hand side (output) arguments
		mxArray *plhs[], // Array of left hand side arguments
		int nrhs, // Number of right hand side (input) arguments
		const mxArray *prhs[] // Array of right hand side arguments
)
{
	double Input, *Output;
	if (nrhs!=1)
        mexErrMsgTxt("One input required");
    if (nlhs>1)
        mexErrMsgTxt("Only one output is supported");
    mrows = mxGetM(prhs[0]);
	ncols = mxGetN(prhs[0]);
	if (!mxIsDouble(prhs[0]) || mxIsComplex(prhs[0]) || !(mrows==1 && ncols==1) )
	{
		mexErrMsgIdAndTxt( "MATLAB:timestwo:inputNotRealScalarDouble",
            "Input must be a noncomplex scalar double.");
	}
	Input = mxGetScalar(prhs[1]);
	//Setup output
	plhs[0] = mxCreateDoubleMatrix(1,1, mxREAL);
	Output = mxGetPr(plhs[0]);
	*Output = IntNoise((int)Input);
}