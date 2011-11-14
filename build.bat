@echo off
call buildvars.bat
del /S /Q .bundle
call bundle install --path vendor
rake.bat build
