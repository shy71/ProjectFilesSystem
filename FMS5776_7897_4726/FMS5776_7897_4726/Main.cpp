#include<iostream>
#include "TestLevel_0.cpp"
using namespace std;
void main()
{
	char arr[12];
	arr[0] = NULL;
	strcpy_s(arr, "shy");
	cout << arr<<endl;
	//strcat_s(arr, "shy");
	cout << arr;
	TestLevel_0 l;
	l.test_0();
	system("pause");
}