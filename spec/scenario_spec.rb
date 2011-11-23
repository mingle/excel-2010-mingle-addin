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

describe "scenarios" do
    def ribbon() @ribbon ||= FakeRibbon.new({:excel => excel}) end
    def excel() @excel ||= FakeExcel.new end

    it "runs multiple queries" do
        query = ribbon.click_fetch_query
        query.select_project(0, 'Foo')
        query.set_query(0, 'SELECT FOO 1')
        query.set_name(0, 'Foo')
        
        query.add_query
        query.select_project(1, 'Bar')
        query.set_query(1, 'SELECT FOO 2')
        query.set_name(1, 'Bar')
        query.click_fetch
        excel.should have_cells([
                                    ['query', 'sprint', 'points'],
                                    ['Foo', '1', '13'],
                                    ['Foo', '2', '8'],
                                    ['Bar', '1', '13'],
                                    ['Bar', '2', '8']
                                ])
        excel.should have_properties([ {:name => 'Foo', :sheet=>'sheet1', :project=>'foo_id', :value=>'SELECT FOO 1'}, 
                                       {:name => 'Bar', :sheet=>'sheet1', :project=>'bar_id', :value=>'SELECT FOO 2'}])
    end
    
    it "saves queries across multiple worksheets" do
        excel.save_properties [ExcelQueryProperty.new('sheet1', 'query0', 'foo_id', 'SELECT BAR 1'),
            ExcelQueryProperty.new('sheet3', 'query0', 'bar_id', 'SELECT FOO 3')].to_ienumerable(ExcelProperty)
    
        query = ribbon.click_fetch_query
        query.select_project(0, 'Foo')
        query.set_query(0, 'SELECT FOO 1')
        query.click_save
        
        excel.choose_sheet 'sheet2'
        
        query = ribbon.click_fetch_query
        query.select_project(0, 'Bar')
        query.set_query(0, 'SELECT FOO 2')
        query.click_save
        
        excel.should have_properties([ {:name => 'query0', :sheet=>'sheet1', :project=>'foo_id', :value=>'SELECT FOO 1'}, 
                                       {:name => 'query0', :sheet=>'sheet3', :project=>'bar_id', :value=>'SELECT FOO 3'}, 
                                       {:name => 'New query', :sheet=>'sheet2', :project=>'bar_id', :value=>'SELECT FOO 2'}])        
    end
end

RSpec::Matchers.define :have_cells do |cells|
  match do |excel|
    sheet = excel.sheet  
    matched = sheet.size == cells.size
    
    sheet.each_in_sync cells do |row, row_cells| 
        matched = false if row.size != row_cells.size
        row.each_in_sync row_cells do |column, cell|
            matched = false if column != cell
        end
    end
    matched
  end
  
  failure_message_for_should do |excel|
    "expected cells #{cells.inspect} but got #{excel.sheet.inspect}"
  end
end

RSpec::Matchers.define :have_properties do |expected|
    match do |excel|
        matched = excel.properties.size == expected.size
        
        excel.properties.each_in_sync expected do |prop, exp|
            exp_prop = ExcelQueryProperty.new(exp[:sheet], exp[:name], exp[:project], exp[:value])
            matched = false if prop.Name != exp_prop.name
            matched = false if prop.Value != exp_prop.value
        end
        matched
    end
    
  failure_message_for_should do |excel|
    "expected properties #{expected.inspect} but got #{excel.properties.inspect}"
  end
end
