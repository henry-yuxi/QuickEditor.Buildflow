#!/usr/bin/python

import os
import sys
import re

print 'start python script! Delete AutoMatically Manage Signing'

filePath = sys.argv[1]+"/Unity-iPhone.xcodeproj/project.pbxproj"

print filePath

f = open(filePath, 'r+')

contents = f.read()

f.seek(0)

f.truncate()

p = re.compile(r'(ProvisioningStyle = Automatic;)')

contents = p.sub(r'ProvisioningStyle = Manual;', contents)

pattern = re.compile(r'(TestTargetID = (\w*));')

f.write(pattern.sub(r'\1;\n\t\t\t\t\t};\n\t\t\t\t\t\2 = {\n\t\t\t\t\t\tProvisioningStyle = Manual;', contents))

f.close()

print 'end python script !'