require File.dirname(__FILE__)+'/spec_helper'

describe 'settings' do
    s = ExcelAddIn::MingleSettings
    it 'should get the host' do
        s.host = 'http://testhost'
        s.host.should == 'http://testhost'
    end

    it 'should get the login' do
        s.login == ''
        s.login.should == ''
    end
  
    it 'should get the password' do
        s.password = 'secret'
        s.password.should == 'secret'
    end
  
  
end