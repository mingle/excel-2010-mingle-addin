@echo off
call buildvars.bat
call bundle install --deployment
rake.bat build
