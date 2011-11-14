require File.dirname(__FILE__) + '/../lib/go_artifacts.rb'
require "bundler/setup"     

describe "stages" do
    it "only gets the latest passing build" do
        Stages.new(feed).get_latest.url.should == 'http://build2'
    end
    
    def feed
    <<-FEED
    <feed xmlns="http://www.w3.org/2005/Atom">
            <entry>
                <id>http://build3</id>
                <category term="failed" />
            </entry>
            <entry>
                <id>http://build2</id>
                <category term="passed" />
            </entry>
            <entry>
                <id>http://build1</id>
                <category term="passed" />
            </entry>
    </feed>
    FEED
    end
end

describe "artifacts scenario" do
    require 'fakefs/spec_helpers'
    include FakeFS::SpecHelpers
    
    it "downloads the artifacts" do
        artifacts = ['one.dll', 'two.dll']
        web = mock('web')
        
        web.should_receive(:get).with('https://go01.thoughtworks.com/go/api/pipelines/Studios-dotNet-API/stages.xml').and_return(feed)
        web.should_receive(:get).with('https://go01.thoughtworks.com/go/files/Studios-dotNet-API/2/build/1/compile-and-test/release.json').and_return(artifacts_json)
        
       artifacts.each do |file|
            web.should_receive(:get).with("https://go01.thoughtworks.com/go/files/Studios-dotNet-API/2/build/1/compile-and-test/release/#{file}").and_return("contents: #{file}")    
        end
        
        ArtifactsDownloader.new('https://go01.thoughtworks.com', 'Studios-dotNet-API', 'compile-and-test', 'release', 'lib').download(web)
        
        artifacts.each do |file|
            File.exists?("lib/#{file}").should be_true
            File.read("lib/#{file}").should == "contents: #{file}"
        end
    end
     
    def feed
    <<-FEED
    <feed xmlns="http://www.w3.org/2005/Atom">
            <entry>
                <id>https://go01.thoughtworks.com/go/pipelines/Studios-dotNet-API/2/build/1</id>
                <category term="passed" />
            </entry>
            <entry>
                <id>https://go01.thoughtworks.com/go/pipelines/Studios-dotNet-API/1/build/1</id>
                <category term="passed" />
            </entry>
    </feed>
    FEED
    end

    def artifacts_json
    <<-JSON
        [{"name": "one.dll"}, {"name": "two.dll"}]
    JSON
    end
end