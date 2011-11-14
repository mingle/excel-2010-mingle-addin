require File.dirname(__FILE__)+'/spec_helper'

describe "queries view" do
    def projects 
        @projects ||= [Project.new('foo_id', 'Foo'), Project.new('bar_id', 'Bar')].
                                    to_ienumerable(Project)
    end

    def queries() @queries ||= Queries.new("sheet1", projects, '') end

    it "deletes queries" do
        query1 = queries.add_query Project.new('proj', 'Project'), 'SELECT BAR', 'Query 1'
        query2 = queries.add_query Project.new('proj', 'Project'), 'SELECT FOO', 'Query 2'
        
        queries.remove query1.delete_button_name
        queries.count.should == 1
    end
    
    it "removes queries correctly after removing and adding them a few times" do
        3.times { |i| queries.add_query }
        
        # remove the middle one, now we have query 0 and 2 left
        queries.remove(queries[1].delete_button_name)
        queries.count.should == 2
        
        # add another, so we have query 0, 2 and now 3
        queries.add_query
        queries.count.should == 3
        
        # remove the middle one (query 2).  If naming is broken this will fail
        queries.remove(queries[1].delete_button_name)
        queries.count.should == 2
    end
    
    describe "defaults" do
        before do
            @new_query = queries.add_query
        end
   
        it "has a default name" do
            @new_query.name.should == 'New query'
        end
        
        it "uses the first project as the default project" do
            @new_query.project.should == projects.first
        end    

        it "has a blank query" do
            @new_query.value.should == ''
        end
    end
end