class Go
    def initialize(url) @url = url end
    
    def get_stages web, project
        feed = web.get("#{@url}/go/api/pipelines/#{project}/stages.xml")
        Stages.new(feed)
    end
    
    def login web, username, password
        web.post("#{@url}/go/auth/security_check", {'j_username' => username, 'j_password' => password})
    end
end

class Stages
    require 'rexml/document'
    
    def initialize(xml) @doc = REXML::Document.new(xml) end

    def get_latest
        last_passed = entries.find { |e| e.get_elements("category[@term='passed']").length != 0 }
        Build.new last_passed.get_text('id').value
    end
    
    def entries
        @doc.get_elements("//entry")
    end
       
    class Build
        attr_reader :url
        
        def initialize(url) @url = url end
        
        def get_artifacts(job, pathname, web)
            json = web.get(list_url(job, pathname))
            Files.new(files_url(job, pathname), json)
        end
        
        def files_url job, pathname
            "#{files_base_url}/#{job}/#{pathname}"
        end

        def files_base_url
            @url.gsub(/pipelines/,"files")
        end
        
        def list_url job, pathname
            "#{files_url(job, pathname)}.json"
        end
        
        class Files
            require 'json/pure'
            
            def initialize(url, json) @url, @json = url, json end
              
            def get_files downloader
                files.each {|f| downloader.download f }
            end
        
            def get_file file, web
                web.get(file_url(file))
            end
                
            def url() @url end
            
            def file_url file
                "#{url}/#{file}"
            end
            
            private
            def files
                files = JSON.parse @json
                files.collect {|f| f['name'] }
            end
        end
    end
end

class ArtifactsDownloader
    def initialize(url, project, job, pathname, download_dir)
        @url, @project, @job, @pathname, @download_dir = url, project, job, pathname, download_dir
    end
    
    def download web
        build_artifacts = latest_build_artifacts(web)
        downloader = Downloader.new(web, build_artifacts, @download_dir)
        build_artifacts.get_files(downloader)
    end
    
    private
    def latest_build_artifacts web
        get_stages(web, @project).get_latest.get_artifacts(@job, @pathname, web)
    end
    
    def get_stages web, project
        Go.new(@url).get_stages(web, project)
    end
    
    class Downloader
        def initialize(web, artifacts, download_dir) @web, @artifacts, @download_dir = web, artifacts, download_dir end
        
        def download file
            contents = @artifacts.get_file file, @web
            File.open("#{@download_dir}/#{file}", "wb") {|w| w.write(contents)}
        end
    end
end