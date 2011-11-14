$LOAD_PATH << File.dirname(__FILE__) + '/../ExcelAddIn/bin/Release'

require 'MingleExcelAddIn.dll'
require 'WpfControls.dll'
include ExcelAddIn
include WpfControls

require File.dirname(__FILE__)+'/fake_view'

class String
    require 'System.Xml.Linq'
    def to_xml
        System::Xml::Linq::XDocument.parse self
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