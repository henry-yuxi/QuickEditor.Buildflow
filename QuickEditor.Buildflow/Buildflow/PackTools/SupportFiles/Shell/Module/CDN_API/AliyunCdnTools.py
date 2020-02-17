#!/bin/env python
# coding: utf-8
# Python 2.x
# pip install aliyun-python-sdk-core aliyun-python-sdk-cdn
# Python 3.x
# pip install aliyun-python-sdk-core-v3 aliyun-python-sdk-cdn

import sys
import json
import argparse
import textwrap
from aliyunsdkcore.client import AcsClient
from aliyunsdkcore.acs_exception.exceptions import ClientException
from aliyunsdkcore.acs_exception.exceptions import ServerException
from aliyunsdkcdn.request.v20180510.RefreshObjectCachesRequest import RefreshObjectCachesRequest

if sys.version_info[0] == 2:
    reload(sys)
    sys.setdefaultencoding('utf-8')

class AliyunCdnTools(object):
    def __init__(self, secretId, secretKey):
        self.secretId = secretId
        self.secretKey = secretKey
        self.client = AcsClient(self.secretId, self.secretKey, 'cn-shanghai')

    def refresh(self, objectpath, type='File'):
        for path in objectpath:
            request = RefreshObjectCachesRequest()
            request.set_accept_format('json')

            request.set_ObjectType(type)
            request.set_ObjectPath(path)
            response = self.client.do_action_with_exception(request)

            if sys.version_info[0] > 2:
                res_json = json.loads(str(response, encoding='utf-8'))
            else:
                res_json = json.loads(response)
            print(res_json)


if __name__ == "__main__":
    parser = argparse.ArgumentParser(
                formatter_class=argparse.RawDescriptionHelpFormatter,
                description="阿里云CDN刷新工具",
                epilog=textwrap.dedent("""\
                    example:
                      {0} -u xxx -p xxx --dirs 'https://res.ylqt.2144gy.com/verify/ANDROID/' --dirs 'https://res.ylqt.2144gy.com/verify/IOS/'
                      {0} -u xxx -p xxx --urls 'https://res.ylqt.2144gy.com/verify/ANDROID/version.txt' --urls 'https://res.ylqt.2144gy.com/verify/IOS/version.txt'
                    """.format(sys.argv[0])))
    parser.add_argument("-u", help=u"secretId，必选项")
    parser.add_argument("-p", help=u"secretKey，必选项")
    parser.add_argument("--dirs", action='append', help=u"批量目录刷新, 结尾需加符号/")
    parser.add_argument("--urls", action='append', help=u"批量URL刷新")
    args = parser.parse_args()

    if args.u and args.p:
        cdntools = AliyunCdnTools(args.u, args.p)
        if args.dirs:
            cdntools.refresh(args.dirs, 'Directory')
        elif args.urls:
            cdntools.refresh(args.urls)
        else:
            print(u"缺少必要参数，请参考--help帮助信息")
    else:
        print(u"缺少必要参数，请参考--help帮助信息")
