import re


def identity(url):
    return re.findall(r"[\w]+[.][A-Za-z]+/", url)[0].replace("/", "")


def baseurl(url):
    return re.findall(r"[htps]+://[\w.]+", url)[0]


print(baseurl("http://www.amazon.com/shopping"))
