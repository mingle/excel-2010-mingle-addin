require File.dirname(__FILE__) + '/../lib/web.rb'

describe "post data" do
    it "converts hashes to string" do
        post_data = PostData.new( {'key1' => 'value1', 'key2' => 'value2', 'key3' => 'value3'} )
        post_data.to_s.should == 'key1=value1&key2=value2&key3=value3'
    end
end
