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

$LOAD_PATH << File.dirname(__FILE__) + '/../ExcelAddIn/bin/Release'

require 'MingleExcelAddIn.dll'
require 'WpfControls.dll'
include ExcelAddIn
include WpfControls

require File.dirname(__FILE__)+'/fake_view'

class String
    require 'System.Xml.Linq'
    def to_xml
        System::Xml::Linq::XElement.parse self
    end
end

def mock_interface(mod)
    Class.new do
        include mod
    end.new
end

module Enumerable
    def each_in_sync(sync_with, &blk)
        self.each_with_index { |item, index|
            blk.call(item, sync_with[index])
        }
    end
    
    def to_ienumerable t
        ls = System::Collections::Generic::List.of(t).new
        self.each {|i| 
            ls.add i
        }
        ls
    end
    
    # IronRuby is missing count
    def count
        self.collect {|d|d}.size        
    end
end