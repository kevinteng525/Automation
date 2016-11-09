require 'watir-webdriver'
require 'watir-webdriver-performance'
require_relative '..\..\Variables\SUP_Variables_Configure.rb'
require_relative '..\..\Objects\Supervisor_Objects.rb'
require_relative '..\..\Objects\SUP_Login_Page.rb'
require_relative '..\..\Objects\SUP_Header_page.rb'
require_relative '..\..\Objects\SUP_Filter_Messages_Page.rb'
require_relative '..\..\Objects\SUP_MessageList_Page.rb'
require_relative '..\..\Objects\SUP_BusinessPolicy_Pane.rb'
require_relative '..\..\Objects\SUP_Preview_Pane.rb'
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
   @data_row1 = @excel_file1.getRow 
   @supervisorweb = SUPSite.new(Watir::Browser.new)
 end
 
 def teardown
   @excel_file1.closeExcel
   @supervisorweb.close
 end
 
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end         
    
  #All the Preview test cases:  
  def test_sup_preview_verify_exchange_message_can_be_previewed_in_the_preview_pane_webid_4921
    sup_preview_testcase(1)
  end  
      
  def test_sup_preview_verify_message_date_time_is_correct_shown_in_the_preview_pane_webid_4925
    sup_preview_testcase(2)
  end        

  def test_sup_preview_verify_BCC_information_can_be_previewed_in_the_preview_pane_webid_4927
    sup_preview_testcase(3)
  end 
  
#===============================================================================================================================================

  #test case
  def sup_preview_testcase(i_row)
    puts"test case: "+i_row.to_s
    #login supervisor web.
    @input_data1 = @excel_file1.getUserData(@data_row1,1)
    @supervisorweb.SUPlogin_page.login_function(@input_data1)
    @supervisorweb.SUPfilter_message_page.close_filter_dialog    
    @supervisorweb.load_time  
    order = "descending"
    @supervisorweb.SUPMessageList_page.SortByColumn(6,order)
    testresult = @supervisorweb.SUPPreview_Pane.checkPreviewContent(i_row)
    if testresult == false
      assert(false, "The message preview is incorrect.")
    else
      assert(true, "The message preview is incorrect.")  
    end     
  end 
   
end
 
