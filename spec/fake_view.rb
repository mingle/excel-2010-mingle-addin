class FakeRibbon
    include IAmARibbon
    
    def initialize opts
        mingle = opts[:mingle] || FakeMingle.new
        @office = MingleOffice.new mingle, opts[:excel], self
    end
    
    def login(host, username, password)
        @office.set_mingle_login_details(LoginWindow::LoginDetails.new(host, username, password))
    end
    
    def set_model(model) 
        @model = model 
        @model.replace_projects [RibbonModel::Project.new('foo_id', 'Foo'), RibbonModel::Project.new('bar_id', 'Bar')].
                                    to_ienumerable(RibbonModel::Project)
    end

    def click_fetch_query
        @model.open_fetch_cards.invoke
        @query_window
    end
    
    def show_query_window queries
        @query_window = FakeQueryWindow.new(queries)
        @queries = queries
    end
    
    def queries
        @queries.collect { |q| q }
    end
    
    def alert_user message
        raise "User shouldn't be alerted but was with: #{message}"
    end
    
    def enable_fetch_query_button
        @fetch_button_enabled = true
    end
    
    def disable_fetch_query_button
        @fetch_button_enabled = false
    end
    
    def is_fetch_button_enabled?
        @fetch_button_enabled != nil or raise "Fetch button not been set"
        @fetch_button_enabled
    end
end

class FakeQueryWindow
    require 'WpfControls.dll'
    def initialize(queries) @queries = queries end
    
    def add_query
        @queries.add_query
    end
    
    def set_name index, name
        @queries.change_name(@queries[index].name_box_name, name)
    end
    
    def select_project index, project
        @queries.change_project(@queries[index].projects_name, project)
    end
    
    def set_query index, query
        @queries.change_query(@queries[index].text_box_name, query)
    end
    
    def click_fetch
        @queries.fetch.invoke
    end
    
    def click_save
        @queries.save.invoke
    end
end

class FakeMingle
    include ITalkToMingle
    
    def has_login_details() true end
    
    def exec_mql project, query
        results.to_xml
    end

    def results
    <<-RESULTS
        <results type="array">
            <result>
                <sprint>1</sprint>
                <points>13</points>
            </result>
            <result>
                <sprint>2</sprint>
                <points>8</points>
            </result>
        </results>
    RESULTS
    end
end

class FakeExcel
    include IAmExcel

    def initialize() @properties, @sheet, @active_sheet = [], [], 'sheet1' end

    def on_workbook_opened(action) end
        
    def get_name_active_sheet
        @active_sheet
    end
    
    def choose_sheet sheet
        @active_sheet = sheet
    end
    
    def populate_sheet_with sheet
        @sheet = sheet
    end
        
    def sheet
        @sheet.collect { |r| r.collect {|c| c.value} }
    end
    
    def properties
        @properties.to_ienumerable(ExcelProperty)
    end
    
    def save_properties properties
        @properties = properties
    end
end
