#include "StaticSearchTable.h"
#include "9_27_binsearch.h"
// #include "LinkTable.h"

#define NUM_OF_TEST 10
#define TEST_EACH 3
#define FULL_MARK 100

/**** IMPORT [SearchBin] ****/
// int Search_Bin(SSTable st, KeyType key){return 0;}

bool check(SSTable st, int ans, KeyType key){
	if(st.length == 0){
		if(ans == 0)
			return true;
		else
			return false;
	}
	if(ans == 0){
		if(st.elem[1].key <= key)
			return false;
		else
			return true;
	}
	if(ans == st.length){
		if(st.elem[st.length].key > key)
			return false;
		else
			return true;
	}
	if(st.elem[ans].key <= key && key < st.elem[ans + 1].key)
		return true;
	return false;
}


int main(){
	/**** ANSWER END ****/
	SSTable st;
	srand((unsigned)time(NULL));
	int i = 0, j = 0, mark = 0, key = 0;
	int test;
	bool result = true;
	// CreateTableSTDIO(t);
	// PrintTable(t);
	// DestroyTable(t);
	for(i = 0; i < NUM_OF_TEST; i++){
		printf("第%d组测试数据：\n", (i + 1));
		// CreateTableSTDIO(st);
		if(i == 0)
			CreateTable(st, 0);
		else
			CreateTable(st);
		PrintTable(st);
		result = true;
		for(j = 0; j < TEST_EACH; j++){
			// printf("Please input the key to search for: ");
			// scanf("%d", &key);
			if(j < TEST_EACH - 1 && j < 1 && st.length > 0)
				key = st.elem[rand() % st.length + 1].key;
			else
				key = rand() % (st.length * 2 + 10);
			printf("查找关键字 %d.\n", key);
			test = -1;
			try{
				test = Search_Bin(st, key);
			}catch(...){
				printf("程序运行出现异常\n");
			}
			if(test == -1){
				printf("程序没有运行结果\n");
				result = false;
			}
			else{
				result = result && check(st, test, key);
				printf("测试： %d\n", test);
			}
		}
		if(result){
			printf("结果：正确\n");
			mark++;
		}
		else
			printf("结果：错误\n");
		DestroyTable(st);
		printf("\n");
	}
	mark = mark * FULL_MARK / NUM_OF_TEST;
	printf("得分：%d\n", mark);
	/**** ANSWER BEGIN ****/
	if(mark == FULL_MARK) printf("CORRECT");
	else printf("INCORRECT");
	/**** ANSWER END ****/
	return 0;
}