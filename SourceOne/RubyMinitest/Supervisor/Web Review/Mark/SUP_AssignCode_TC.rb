require 'watir-webdriver'
require 'watir-webdriver-performance'
require_relative '..\..\Variables\SUP_Variables_Configure.rb'
require_relative '..\..\Objects\Supervisor_Objects.rb'
require_relative '..\..\Objects\SUP_Login_Page.rb'
require_relative '..\..\Objects\SUP_Header_page.rb'
require_relative '..\..\Objects\SUP_Filter_Messages_Page.rb'
require_relative '..\..\Objects\SUP_MessageList_Page.rb'
require_relative '..\..\Objects\SUP_BusinessPolicy_Pane.rb'
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
   #get the data from sheet #4 of mark
   @excel_file4 = ReadExcel.new($excel_dir,4)
   @data_row1 = @excel_file1.getRow 
   @data_row4 = @excel_file4.getRow
   @supervisorweb = SUPSite.new(Watir::Browser.new)
 end
 
 def teardown
   @excel_file1.closeExcel
   @excel_file4.closeExcel
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
   
  #All the Assign Code test cases:  
  def test_sup_assign_13_verify_show_correct_status_if_mark_as_completely_code_webid_5047
    sup_assign_testcase(13)
  end 
      
  def test_sup_assign_14_verify_show_correct_status_if_mark_as_partially_code_webid_5046
    sup_assign_testcase(14)
  end 

  def test_sup_assign_15_verify_add_comments_while_mark_single_message_webid_5052
    sup_assign_testcase(15)
  end 

  def test_sup_assign_16_verify_mark_as_OK_for_one_of_the_BPs_of_this_message_webid_5050
    sup_assign_testcase(16)
  end 

  def test_sup_assign_17_message_has_only_one_BP_assign_completely_code_webid_5222
    sup_assign_testcase(17)
  end  

  def test_sup_assign_18_message_has_only_one_BP_assign_partially_code_webid_5223
    sup_assign_testcase(18)
  end 

  def test_sup_assign_19_message_has_several_BPs_assign_completely_code_webid_5227
    sup_assign_testcase(19)
  end 

  def test_sup_assign_20_message_has_several_BPs_assign_partially_code_webid_5228
    sup_assign_testcase(20)
  end 
        
#===============================================================================================================================================

  #test case
  def sup_assign_testcase(i_row)
    #login supervisor web.
    puts "test case "+i_row.to_s
    @input_data1 = @excel_file1.getUserData(@data_row1,1)
    @supervisorweb.SUPlogin_page.login_function(@input_data1)
    @supervisorweb.SUPfilter_message_page.close_filter_dialog    
    @supervisorweb.load_time
    @input_data4 = @excel_file4.getUserData(@data_row4,i_row)    
    testreuslt = @supervisorweb.SUPBusinessPolicy_Pane.assigncode(@input_data4)
      if testreuslt == false
        assert(false, "The added comment and mark are incorrect.")
      else
        assert(true, "The added comment and mark are as expected.")  
      end         
  end 
   
end
 
