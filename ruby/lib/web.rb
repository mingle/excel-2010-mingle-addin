require 'mscorlib' 
require 'System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' 
include System

class CookieAuthenticatedWeb
    def initialize(username, password, go) @username, @password, @go = username, password, go end
    
    def get location
        wrapped.get location
    end
    
    def post location, data
        wrapped.post location, data
    end
    
    def wrapped
        @wrapped ||= create_and_logon
    end
    
    def create_and_logon
        web = AuthenticatedWeb.new(@username, @password)
        @go.login(web, @username, @password)
        web
    end
end

class AuthenticatedWeb
    def initialize(username, password) @credentials = Net::NetworkCredential.new(username, password) end

    def get location
        request = create_request "get", location
        puts "Getting #{location}"
        begin          
            response = request.GetResponse()
            String.new(System::IO::StreamReader.new(response.GetResponseStream()).ReadToEnd())
	    ensure
            response.close if response
	    end
    end
    
    def post location, data
        request = create_request "post", location
        request.ContentType = "application/x-www-form-urlencoded"
     
        encodedBytes = System::Text::Encoding.UTF8.GetBytes(PostData.new(data).to_s);

        begin
            stream = request.GetRequestStream()
            stream.Write(encodedBytes, 0, encodedBytes.Length);
        ensure
            stream.close if stream
        end
        
        response = request.GetResponse
        response.close
    end
    
    private
    def create_request method, location
        request = Net::WebRequest.Create(location)
        request.CookieContainer = cookie_jar;
        request.Method = method
        request.Credentials = @credentials
	    request.PreAuthenticate = true
        request
    end
    
    def cookie_jar
        @cookie_jar ||= Net::CookieContainer.new
    end
end

class PostData
    def initialize(data) @data = data end
    
    def to_s
        first = @data.shift
        post_data = "#{first[0]}=#{first[1]}"
        @data.each {|k,v| post_data += "&#{k}=#{v}" }
        post_data
    end
end
