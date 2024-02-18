/* Justinas Linkevicius
 * IFF-3/2
 * L1b
 *

1. Kokia tvarka startuoja procesai?
** atsitiktine

2. Kokia tvarka vykdomi procesai?
** atsitiktine

3. Kiek iteracijų iš eilės padaro vienas procesas?
** atsitiktinį skaičių

4. Kokia tvarka atspausdinami to paties masyvo duomenys?
**  atsitiktine.

*/

#include <stdio.h>
#include <string>
#include <iostream>
#include <fstream>
#include <vector>
#include <omp.h>

using namespace std;

#define OUTPUTFILE "C:\\Users\\pc\\Downloads\\lab1_bfnl1\\lab1_bfnl1\\fnllab1_b\\fnllab1_b\\Text1.txt"

#define INPUTFILE "C:\\Users\\pc\\Downloads\\lab1_bfnl1\\lab1_bfnl1\\fnllab1_b\\fnllab1_b\\Text.txt"

#define THREADS 6


struct Student
{
	string  stringField;
	int          intField;
	double       doubleField;

	Student()
	{
		stringField = "";
		intField = 0;
		doubleField = 0.0;
	}

	Student(string a, int b, double c)
	{
		stringField = a;
		intField = b;
		doubleField = c;
	}
};

class ResultMonitor {
public:
	Student students[25];
	int n;

	ResultMonitor()
	{
		n = 0;
	}

	void AddItem(Student d) {

		omp_lock_t lock;

		omp_init_lock(&lock);


#pragma omp parallel num_threads(THREADS)
		{
			omp_set_lock(&lock);

			if (d.intField > 1) {
				students[n] = d;
				n++;
			}
			omp_unset_lock(&lock);
		}

		omp_destroy_lock(&lock);
	};
	//void Print() {
	//	for (auto i : students) {
	//		//cout << i->intField << "\n";
	//		printf("_%d", i.intField);
	//	}
	//};
};

string stringFieldName, intFieldName, doubleFieldName;

int dataElementsCount;

int threadDataSize;

void readFile(vector<Student*>& threadDataArray)
{
	ifstream input(INPUTFILE);

	input >> stringFieldName;
	input >> intFieldName;
	input >> doubleFieldName;
	input >> dataElementsCount;

	threadDataSize = ceil((double)dataElementsCount / THREADS);

	int line = 0;
	for (int i = 0; i < THREADS; i++)
	{
		Student* threadData = new Student[threadDataSize];

		for (int j = 0; j < threadDataSize; j++)
		{
			string stringField;
			int intField;
			double doubleField;

			input >> stringField >> intField >> doubleField;

			if (line < dataElementsCount)
				threadData[j] = Student(stringField, intField, doubleField);
			else
				threadData[j] = Student();

			line++;
		}

		threadDataArray.push_back(threadData);
	}
	input.close();
}

void writeData(vector<Student*> threadDataArray, string text)
{
	ofstream textfile;
	textfile.open(text);
	int line = 0;
	textfile << stringFieldName << "\t" << intFieldName << "\t" << doubleFieldName << "\r\n";
	for (int i = 0; i < THREADS; i++)
	{
		textfile << endl << "** Array" << i << " **" << endl;
		for (int j = 0; j < threadDataSize; j++)
		{
			line++;

			if (threadDataArray[i][j].stringField != "")
			{
				cout.precision(2);
				textfile << j << ") " << threadDataArray[i][j].stringField << "\t" << threadDataArray[i][j].intField << "\t" << fixed << threadDataArray[i][j].doubleField << "\r\n";
			}

			if (line == dataElementsCount)
				break;
		}
	}
	textfile.close();
	cout << endl;
}


void doThreadWork(int threadId, Student* threadData, ResultMonitor rm1)
{
	for (int i = 0; i < threadDataSize; i++)
	{
		if (threadData[i].stringField != "")
		{
			printf("process_%d: %d\t%s\t%d\t%.2f\n", threadId, i, threadData[i].stringField.c_str(), threadData[i].intField, threadData[i].doubleField);
			rm1.AddItem(threadData[i]);
		}
	}
}

void startMultithreading(vector<Student*> threadDataArray, ResultMonitor rm1)
{
	omp_set_num_threads(THREADS);

	printf("Start Multithreading!\n");
	int threadId;

#pragma omp parallel private(threadId) 
	{
		threadId = omp_get_thread_num();

		doThreadWork(threadId, threadDataArray[threadId], rm1);
	};
	printf("End multithreading!\n");
};

int main(int argc, char* argv[])
{
	ResultMonitor rm1;
	vector<Student*> threadDataArray;
	readFile(threadDataArray);
	writeData(threadDataArray, OUTPUTFILE);
	startMultithreading(threadDataArray, rm1);
	cout << rm1.n;
	std::cin.get();
	return 0;
};