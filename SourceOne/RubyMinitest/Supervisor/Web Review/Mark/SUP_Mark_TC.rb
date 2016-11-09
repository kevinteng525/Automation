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
=begin  
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
=end   
  #All the Mark test cases:  
  def test_sup_mark_01_completely_code_one_BP_webid_5206
    sup_mark_testcase(1)
  end  

  def test_sup_mark_02_partially_code_one_BP_webid_5207
    sup_mark_testcase(2)
  end  

  def test_sup_mark_03_partially_code_for_all_BPs_webid_5216
    sup_mark_testcase(3)
  end 

  def test_sup_mark_04_completely_code_for_all_BPs_webid_5215
    sup_mark_testcase(4)
  end 

  def test_sup_mark_05_partially_code_for_one_BPs_webid_5212
    sup_mark_testcase(5)
  end  

  def test_sup_mark_06_completely_code_for_one_BPs_webid_5211
    sup_mark_testcase(6)
  end
  
  def test_sup_mark_07_completely_add_comments_separately_for_all_BPs_webid_5217
    sup_mark_testcase(7)
  end
  
  def test_sup_mark_08_verify_show_correct_status_if_mark_as_completely_webid_5037
    sup_mark_testcase(8)
  end    

  def test_sup_mark_09_verify_show_correct_status_if_mark_as_partially_webid_5036
    sup_mark_testcase(9)
  end 

  def test_sup_mark_10_verify_add_comments_while_mark_single_message_webid_5042
    sup_mark_testcase(10)
  end 

  def test_sup_mark_11_verify_mark_OK_for_one_of_BPs_webid_5040
    sup_mark_testcase(11)
  end 

  def test_sup_mark_12_verify_mark_OK_for_all_of_BPs_webid_5041
    sup_mark_testcase(12)
  end 
   
  def test_sup_mark_select_several_messages_which_have_different_BPs_mark_completely_code_for_one_of_BPs_webid_5250
    sup_mark_selected_testcase(21)
  end 

  def test_sup_mark_select_several_messages_which_have_different_BPs_mark_partially_code_for_one_of_BPs_webid_5253
    sup_mark_selected_testcase(22)
  end  

  def test_sup_mark_23_add_comments_when_mark_selected_webid_5062
    sup_mark_selected_testcase(23)
  end
 
  def test_sup_mark_24_Show_correct_status_if_mark_as_partially_code_webid_5063
    sup_mark_selected_testcase(24)
  end   
        
#===============================================================================================================================================

  #test case
  def sup_mark_testcase(i_row)
    #login supervisor web.
    @input_data1 = @excel_file1.getUserData(@data_row1,1)
    @supervisorweb.SUPlogin_page.login_function(@input_data1)
    @supervisorweb.SUPfilter_message_page.close_filter_dialog    
    @supervisorweb.load_time
    @input_data4 = @excel_file4.getUserData(@data_row4,i_row)    
    testreuslt = @supervisorweb.SUPMessageList_page.markMessage(@input_data4)
      if testreuslt == false
        assert(false, "The added comment and mark are incorrect.")
      else
        puts "Test case "+ i_row.to_s+" is Pass!"
        assert(true, "The added comment and mark are as expected.")  
      end         
  end 
  
  def sup_mark_selected_testcase(i_row) 
    #login supervisor web.
    @input_data1 = @excel_file1.getUserData(@data_row1,1)
    @supervisorweb.SUPlogin_page.login_function(@input_data1)
    @supervisorweb.SUPfilter_message_page.close_filter_dialog    
    @supervisorweb.load_time
    @input_data4 = @excel_file4.getUserData(@data_row4,i_row)
    testreuslt = @supervisorweb.SUPMessageList_page.markSelectedMessages(@input_data4)
      if testreuslt == false
        assert(false, "The added comment and mark selected messages are incorrect.")
      else
        puts "Test case "+ i_row.to_s+" is Pass!"
        assert(true, "The added comment and mark selected messages are as expected.")  
      end  
  end
end
 
