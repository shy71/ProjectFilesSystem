#include<iostream>
#include "TestLevel_0.cpp"
using namespace std;
void main()
{
	Disk d;
	string str;
	cout << "enter name" << endl;
	cin >> str;
	d.createDisk(str, "shy");
	d.mountDisk(str);
	cout << "file name" << endl;
	system("pause");
	d.createfile("file1", "shy", "F", 8, 10, "I", 0, 4);
	FCB *fcb=d.openfile("file1", "shy", "IO");
		system("pause");
	//TestLevel_0 l;

	//l.test_0();
	//Test_Level_1 l1;
	//l1.CheckFunctions("aas", "oshri");
	////system("del disk1");
	//system("pause");
}