#ifndef STATICSEARCHTABLE_H
#define STATICSEARCHTABLE_H

#include <cstdio>
#include <algorithm>
#include <ctime>
#include <cstdlib>

using namespace std;

// ******** type definition ********* //~

#define MAX_SIZE 20

typedef int KeyType;
typedef int DataType;

typedef struct{
	KeyType key;
	DataType data;
}ElemType;

typedef struct{
	ElemType *elem;
	int length;
}SSTable;

// ********* function declaration ********* //~

void CreateTableSTDIO(SSTable &t);
void CreateTable(SSTable &t);
void CreateTable(SSTable &t, int length);
void PrintTable(SSTable t);
void DestroyTable(SSTable &t);

// ********* function implementation ********* //~

void CreateTableSTDIO(SSTable &t){
	printf("Please input the size of the table: ");
	scanf("%d", &t.length);
	if(t.length <= 0)
		t.length = 0;
	t.elem = (ElemType*)malloc(sizeof(ElemType) * (t.length + 1));		// start from the 1st element
	KeyType *temp = (KeyType*)malloc(sizeof(KeyType) * (t.length + 1));	// store the seq temperarily
	int i = 0;
	printf("Now please input the elements of the table: ");
	for(i = 1; i <= t.length; i++) scanf("%d", &temp[i]);
	sort(temp + 1, temp + t.length + 1);
	for(i = 1; i <= t.length; i++) t.elem[i].key = temp[i];
	t.elem[0].key = -1;
}

void CreateTable(SSTable &t){
	int unit = MAX_SIZE / 3;
	if(unit == 0) unit = 1;
	int length = rand() % (2 * unit);
	length += unit;
	CreateTable(t, length);
}

void CreateTable(SSTable &t, int length){
	if(length <= 0)
		t.length = 0;
	else
		t.length = length;
	t.elem = (ElemType*)malloc(sizeof(ElemType) * (t.length + 1));
	KeyType *temp = (KeyType*)malloc(sizeof(KeyType) * (t.length + 1));
	int i = 0;
	for(i = 1; i <= length; i++)	temp[i] = rand() % (length * 2);
	sort(temp + 1, temp + length + 1);
	for(i = 1; i <= length; i++) t.elem[i].key = temp[i];
	t.elem[0].key = -1;
}

void PrintTable(SSTable t){
	printf("查找表中共有%d个元素，依次为：\n", t.length);
	int i = 0;
	for(i = 1; i <= t.length; i++) printf("%d ", t.elem[i].key);
	printf("\n");
}

void DestroyTable(SSTable &t){
	free(t.elem);
	t.length = 0;
}

#endif