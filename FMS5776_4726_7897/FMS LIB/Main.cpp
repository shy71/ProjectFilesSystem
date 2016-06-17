#include<iostream>
#include "TestLevel_0.cpp"
using namespace std;
class Record8
{
	unsigned int id;
	char name[4];
public:
	Record8(){}
	Record8(unsigned int i, char * n)
	{
		id = i;
		strcpy(name, n);
	}
};
void main()
{
	Disk d;
	string str;
	cout << "enter name" << endl;
	cin >> str;
	//d.createDisk(str, "shy");
	d.mountDisk(str);
	cout << "file name" << endl;
	system("pause");
	//d.createfile("file1", "shy", "F", 8, 10, "I", 0, 4);
	FCB *fcb=d.openfile("file1", "shy", "IO");
	//fcb->addRecord((char*)(new Record8(207447897, "shy")));
	//fcb->addRecord((char*)(new Record8(327458321, "ezr")));
	Record8 rec;
	fcb->read((char *)&rec, 1);
	fcb->deleteRecord();

		system("pause");
	//TestLevel_0 l;

	//l.test_0();
	//Test_Level_1 l1;
	//l1.CheckFunctions("aas", "oshri");
	////system("del disk1");
	//system("pause");
}