# **tools**

_my project use tools_

## AppFtpUnload

unload a file to all lan ftp server of specifies port.

```
AppFtpUnload -f|--file path -ip ip -port port -?|-h|--help
-f|--file path      local file path
-ip ip              local lan ip [192.168.1.1]
-port port          ftp server port
-?|-h|--help        show this message
```

<i>sample</i>

```
AppFtpUnload -f c:\test.apk -port 2121
```

## VersionMaker

generate a revision file on the basis of git or svn file system

_config.txt file format_

```
D:\project\client\game\bin\revision.txt     | revision file path
2                                           | get info from several dirs
git,D:\project\client\game\bin              | git or svn,dir,head name
svn,D:\project\client\game\bin\data,data    | git or svn,dir,head name
```
