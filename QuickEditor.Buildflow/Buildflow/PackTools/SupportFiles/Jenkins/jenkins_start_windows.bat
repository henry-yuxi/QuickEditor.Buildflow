@echo off
# Windos环境下命令行启动Jenkins

set JENKINS_HOME=D:\Program Files\Jenkins
d:
cd /d %JENKINS_HOME%
net stop jenkins
java -jar jenkins.war --ajp13Port=-1 --httpPort=8088
@cmd.exe