#
#   Copyright 2011 ThoughtWorks, Inc.

#   Licensed under the Apache License, Version 2.0 (the "License"); 
#   you may not use this file except in compliance with the License. 
#   You may obtain a copy of the License at:

#   http://www.apache.org/licenses/LICENSE-2.0

#   Unless required by applicable law or agreed to in writing, software 
#   distributed under the License is distributed on an "AS IS" BASIS, 
#   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
#   See the License for the specific language governing permissions and 
#   limitations under the License.

require File.dirname(__FILE__)+'/spec_helper'

describe "mingle office" do
    def ribbon() @ribbon ||= FakeRibbon.new( {:excel => excel, :mingle => mingle} ) end
    def excel() @excel ||= mock_interface(IAmExcel) end
    def mingle() @mingle ||= mock_interface(ITalkToMingle) end
       
    DEFAULT_QUERY = { :project => 'foo_id', :query => '' }
    
    before do
        excel.stub(:get_name_active_sheet).and_return('sheet1')
        mingle.stub(:has_login_details).and_return(true)
        mingle.stub(:set_login_details)
        
        def excel.on_workbook_opened(action)
            action.invoke
        end
    end
    
    it "provides a default query" do
        excel.stub(:properties).and_return([].to_ienumerable(ExcelProperty))    
        ribbon.click_fetch_query
        ribbon.should have_queries([
                                DEFAULT_QUERY
                              ])
    end
    
    it "loads queries from excel" do
        excel.stub(:properties).and_return(
            [ExcelQueryProperty.new('sheet1', 'first query', 'foo_id', 'SELECT BAR From FOO'),
            ExcelQueryProperty.new('sheet1', 'second query', 'foo_id', 'SELECT FOO From BAR')].to_ienumerable(ExcelProperty))
        
        ribbon.click_fetch_query
        ribbon.should have_queries([
                                {:name=>'first query', :project => 'foo_id', :query => 'SELECT BAR From FOO'},
                                {:name=>'second query', :project => 'foo_id', :query => 'SELECT FOO From BAR'}
                              ])
    end
    
    it "ignores queries from other sheets" do
        excel.stub(:properties).and_return(
            [ExcelQueryProperty.new('sheet1', 'query1', 'foo_id', 'SELECT FOOBAR From BARFOO'),
            ExcelQueryProperty.new('sheet2', 'query1', 'foo_id', 'SELECT BAR From FOO'),
            ExcelQueryProperty.new('sheet2', 'query2', 'bar_id', 'SELECT FOO From BAR')].to_ienumerable(ExcelProperty))
            
        ribbon.click_fetch_query
        ribbon.should have_queries([
                                {:project => 'foo_id', :query => 'SELECT FOOBAR From BARFOO'},
                              ])
                              
        excel.stub(:get_name_active_sheet).and_return('sheet2')
        ribbon.click_fetch_query
        ribbon.should have_queries([
                                {:project => 'foo_id', :query => 'SELECT BAR From FOO'},
                                {:project => 'bar_id', :query => 'SELECT FOO From BAR'}
                              ])
    end
    
    it "ignores non-query document properties" do
        excel.stub(:properties).and_return(
            [ExcelProperty.new('Something', 'Should not appear'),
            ExcelProperty.new('Another Thing', 'Nothing to do with you')].to_ienumerable(ExcelProperty))
            
        ribbon.click_fetch_query
        ribbon.should have_queries([
                                DEFAULT_QUERY
                              ])
    end
    
    describe "fetch button" do
        it "is disabled on first load" do
            mingle.stub(:has_login_details).and_return false
            ribbon.is_fetch_button_enabled?.should be_false
        end
    
        it "is enabled after providing login details" do
            ribbon.login 'http://mingle', 'user', 'password'
            ribbon.is_fetch_button_enabled?.should be_true
        end
        
        it "is enabled if login settings have been saved" do
            mingle.stub(:has_login_details).and_return true
            ribbon.is_fetch_button_enabled?.should be_true
        end
    end
end

RSpec::Matchers.define :have_queries do |expected|
    match do |ribbon|
        check(ribbon.queries, expected)
    end
    
    def check(queries, expected)
        return false if queries.size != expected.size
        queries.each_in_sync expected do |q, e|
            return false if q.value != e[:query] || q.project.id != e[:project]
            return false if e[:name] && q.name != e[:name]
        end
        true
    end
    
    failure_message_for_should do |ribbon|
        "expected #{expected.inspect} but got #{ribbon.queries.inspect}"
    end
end