#!/usr/bin/python
 # -*- coding: utf-8 -*-
import json
import requests
import time

url = "http://localhost:61070/api/judge"

def read_json():
    x = requests.get(url)
    return x.json() if x.status_code == 200 else None


def post_json(res):
    r = requests.post(url, res)


def TEST_RES(x):
    return  {
                "SubmitID":x,  # 提交记录ID
                "State": "Accepted",  #评测结果
    			"Score":"15",
                "CompilerRes":"编译器返回结果",
                "result":json.dumps([
                    [10,"正确",0.4,18],
					[5,"部分正确",0.4,18],
					[0,"超出时间限制",1.2,18],
					[0,"超出内存限制",0.4,1800],
					[0,"程序崩溃(栈溢出)",0.4,18],
                ]),  # 结果详情
            }
		

def TEST_RES(x):		
    return {
                "SubmitID":x,  # 提交记录ID
                "State": "CompileError",  #评测结果
				"Score":"0",
                "CompilerRes":"编译器返回结果",
                "result":"[]",  # 结果详情
            }
			

def Main():
    while True:
        x = read_json()
        if x["SubmitID"] != 0:
            print "Get !" , x["SubmitID"]
            ret = TEST_RES(x["SubmitID"])
            post_json(ret)
        else:
            print "Wait"
            time.sleep(1)


if __name__=="__main__":
    Main()