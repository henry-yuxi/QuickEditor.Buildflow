#!/usr/bin/python
# -*- coding: utf-8 -*-

import os,sys,string,datetime,time,threading

g_bStop = False

class OutputLogThread(threading.Thread):
    m_logFilePath = ''
    def run(self):
        global g_bStop
        nPosRead = 0
        fp = None
        print 'OutputLogThread Start'
        while g_bStop == False:
            if os.path.isfile(self.m_logFilePath):
                if fp == None:
                    fp = open(self.m_logFilePath, 'r')
            if fp != None:
                fp.seek(nPosRead)
                allLines = fp.readlines()
                nPosRead = fp.tell()
                fp.close()
                fp = None
                for lines in allLines:
                    print lines
            time.sleep(0.5)
    def __init__(self, logPath):
        threading.Thread.__init__(self)
        self.m_logFilePath = logPath

if __name__ == '__main__':
	if len(sys.argv) < 2:
		print 'not find unity path'
		sys.exit(-1)
	logFilePath = 'editor.txt'
	unityRunParm = ''
	for i in range(len(sys.argv)):
		if i > 0:
			unityRunParm += ' ' + sys.argv[i]
	unityRunParm += ' -logfile ' + logFilePath
	if os.path.isfile(logFilePath):
		os.remove(logFilePath)
	logThread = OutputLogThread(logFilePath)
	logThread.start()
	os.system(unityRunParm)
	g_bStop = True
	logThread.join()

