#include<iostream>
#include<fstream>

using namespace std;

struct shy
{
	int a;
	char b;
	char arr[12];
	unsigned int t;
};

struct avi
{
	int a;
	char b;
	char arr[12];
	unsigned int t;
	int a1;
	char b1;
	char arr1[12];
	unsigned int t1;
};

void main()
{
	cout << sizeof(shy) << endl;
	cout << sizeof(avi) << endl;
	shy s,d;
	s.a = 7;
	s.b = 'b';
	strcpy_s(s.arr,"shy tennenb");
	s.t = 58643;

	char *arr=new char[200];
	ofstream file("text.txt", ios::out | ios::binary);
	file.write(reinterpret_cast<char *>(&s),sizeof(avi));
	file.close();
	ifstream file2("text.txt", ios::in | ios::binary);
	file2.read(arr, sizeof(avi));
	d = *(reinterpret_cast<shy*>(arr));

	system("pause");
}