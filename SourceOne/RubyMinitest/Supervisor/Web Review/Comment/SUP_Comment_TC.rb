require 'watir-webdriver'
require 'watir-webdriver-performance'
require_relative '..\..\Variables\SUP_Variables_Configure.rb'
require_relative '..\..\Objects\Supervisor_Objects.rb'
require_relative '..\..\Objects\SUP_Login_Page.rb'
require_relative '..\..\Objects\SUP_Header_page.rb'
require_relative '..\..\Objects\SUP_Filter_Messages_Page.rb'
require_relative '..\..\Objects\SUP_MessageList_Page.rb'
require_relative '..\..\Common\Data_Read.rb'
require_relative '..\..\Common\Date_Verify.rb'

gem "minitest"
require 'minitest/autorun'
#require_relative '..\Objects\minitest_saber_base.rb'

#Smoke test cases.
class TestSuite < Minitest::Test  
 
 def setup
   #get the data from sheet #1 of login user
   @excel_file1 = ReadExcel.new($excel_dir,1)
   #get the data from sheet #3 of comment
   @excel_file3 = ReadExcel.new($excel_dir,3)
   @data_row1 = @excel_file1.getRow 
   @data_row3 = @excel_file3.getRow
   @supervisorweb = SUPSite.new(Watir::Browser.new)
 end
 
 def teardown
   @excel_file1.closeExcel
   @excel_file3.closeExcel
   @supervisorweb.close
 end
 
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end 
      
  #prepare
  def test_prepare
    #get the data from sheet #2 of filter
    @excel_file2 = ReadExcel.new($excel_dir,2)
    @data_row2 = @excel_file2.getRow
    #login supervisor web.
    @input_data1 = @excel_file1.getUserData(@data_row1,1)
    @supervisorweb.SUPlogin_page.login_function(@input_data1)
    #set the conditions on the filter dialog box.
    @input_data2 = @excel_file2.getUserData(@data_row2,1)
    @supervisorweb.SUPfilter_message_page.setShowCondition(@input_data2)      
    @excel_file2.closeExcel    
  end
     
  #All the Comment test cases:  
  def test_sup_comment_add_comment_for_a_message_webid_5072
    sup_add_comment_testcase(1)
  end 

  def test_sup_comment_add_comment_for_selected_messages_webid_5081
    sup_add_comment_testcase(2)
  end 

  def test_sup_comment_add_comment_for_all_message_webid_5331
    sup_add_comment_testcase(3)
  end 

  def test_sup_comment_apply_remaining_items_webid_5082
    sup_add_comment_testcase(4)
  end  
      
#===============================================================================================================================================

  #test case
  def sup_add_comment_testcase(i_row)
    puts "test case "+ i_row.to_s
    #login supervisor web.
    @input_data1 = @excel_file1.getUserData(@data_row1,1)
    @supervisorweb.SUPlogin_page.login_function(@input_data1)
    @supervisorweb.SUPfilter_message_page.close_filter_dialog    
    @supervisorweb.load_time
    @input_data3 = @excel_file3.getUserData(@data_row3,i_row)
    if @input_data3[0] =="all"
      #comment all.
      allresult = @supervisorweb.SUPMessageList_page.commentAll(@input_data3)
      #verify random row's comment.
      if allresult.include?"false"
        assert(false, "The content of the added comment is incorrect.")
      else
        assert(true, "The content of the added comment is expected.")
      end
    else
      testreuslt = @supervisorweb.SUPMessageList_page.addComment(@input_data3)
      if testreuslt == false
        assert(false, "The content of the added comment is incorrect.")
      else
        assert(true, "The added comment is as expected.")  
      end         
    end
  end 
 
   
end
 
