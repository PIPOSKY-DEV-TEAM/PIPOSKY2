#include "StaticSearchTable.h"

/*
int Search_Bin(SSTable st, KeyType key){
	if(st.length <= 0)
		return 0;
	int low = 1, high = st.length, mid = 0;
	if(key < st.elem[low].key)
		return 0;
	if(key >= st.elem[high].key)
		return high;
	while(low < high){
		if(high - low == 1)
			return low;
		mid = (high + low) / 2;
		if(key < st.elem[mid].key)
			high = mid;
		else
			low = mid;
	}
}
*/

int STDSearch(SSTable st, KeyType key){
	int i;
	for(i = 1; i <= st.length && st.elem[i].key <= key; i++);
	return (i - 1);
}