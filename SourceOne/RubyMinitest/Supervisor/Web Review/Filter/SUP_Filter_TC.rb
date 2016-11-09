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
   #get the data from sheet #2 of filter
   @excel_file2 = ReadExcel.new($excel_dir,2)
   @data_row1 = @excel_file1.getRow 
   @data_row2 = @excel_file2.getRow 
   @supervisorweb = SUPSite.new(Watir::Browser.new)
 end
 
 def teardown
   @excel_file1.closeExcel 
   @excel_file2.closeExcel 
   @supervisorweb.close
 end
 
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end       
    

  #All the Filter test cases:  
  def test_sup_filter_dialog_popup_when_starts_webid_5297
    sup_filter_testcase(1)
  end 

  def test_sup_filter_messages_can_filter_by_valid_message_dated_webid_5312
    
    sup_filter_testcase(2)
  end 

  def test_sup_filter_messages_cannot_filter_by_invalid_message_dated_webid_5314
    sup_filter_testcase(3)
  end 

  def test_sup_filter_messages_can_filter_by_review_complete_messages_dated_within30day_webid_5310
    sup_filter_testcase(4)
  end 

  def test_sup_filter_messages_can_filter_by_escalated_messages_webid_5311
    sup_filter_testcase(5)
  end 

  def test_sup_filter_messages_can_filter_by_review_incomplete_messages_dated_allmessages_webid_5299
    sup_filter_testcase(6)
  end 

  def test_sup_filter_messages_can_filter_by_review_incomplete_messages_dated_within30day_webid_5304
    sup_filter_testcase(7)
  end 

  def test_sup_filter_messages_can_filter_by_review_incomplete_messages_dated_within1day_webid_5300
    sup_filter_testcase(8)
  end 

  def test_sup_filter_messages_can_filter_by_review_incomplete_messages_dated_within5day_webid_5302
    sup_filter_testcase(9)
  end 

  def test_sup_filter_messages_can_filter_by_review_incomplete_messages_dated_within3day_webid_5301
    sup_filter_testcase(10)
  end 

  def test_sup_filter_messages_can_filter_by_review_incomplete_messages_dated_within15day_webid_5303
    sup_filter_testcase(11)
  end 

  def test_sup_filter_messages_can_filter_by_review_complete_messages_dated_allmessages_webid_5305
    sup_filter_testcase(12)
  end 

  def test_sup_filter_messages_can_filter_by_review_complete_messages_dated_within3day_webid_5307
    sup_filter_testcase(13)
  end 

  def test_sup_filter_messages_can_filter_by_review_complete_messages_dated_within15day_webid_5309
    sup_filter_testcase(14)
  end 

  def test_sup_filter_messages_can_filter_by_review_complete_messages_dated_within1day_webid_5306
    sup_filter_testcase(15)
  end 

  def test_sup_filter_messages_can_filter_by_review_complete_messages_dated_within5day_webid_5308
    sup_filter_testcase(16)
  end 

  def test_sup_filter_messages_cannot_filter_by_invalid_message_dated_abc_webid_5315
    sup_filter_testcase(17)
  end 

  def test_sup_filter_messages_automatically_disappear_uncheck_review_incomplete_webid_5321
    sup_filter_testcase(18)
  end 

  def test_sup_filter_messages_automatically_disappear_uncheck_escalated_message_webid_5322
    sup_filter_testcase(19)
  end

  def test_sup_filter_messages_automatically_disappear_uncheck_review_complete_webid_5320
    sup_filter_testcase(20)
  end

  def test_sup_filter_messages_all_the_review_group_webid_5316
    sup_filter_testcase(21)
  end 
  
  def test_sup_filter_messages_any_combination_of_filter_options_webid_5319
    sup_filter_testcase(22)
  end 

  def test_sup_filter_messages_some_review_groups_webid_5318
    sup_filter_testcase(23)
  end 

  def test_sup_filter_messages_cannot_filter_startdate_greater_enddate_webid_5313
    sup_filter_testcase(24)
  end         
#===============================================================================================================================================

  #test case
  def sup_filter_testcase(i_row)
      puts "test case: "+i_row.to_s
      #login supervisor web.
      @input_data1 = @excel_file1.getUserData(@data_row1,1)
      @supervisorweb.SUPlogin_page.login_function(@input_data1)
      #verify filter dialog box can pop up when login.
      if @supervisorweb.SUPfilter_message_page.verifyFilterDialog == true
        assert(true, message="Filter dialog box can be found.")
      else
        assert(false, message="error: Filter dialog box cannot be found.")
      end
      #set the conditions on the filter dialog box.
      returned = configure_message_conditon(i_row)
      @supervisorweb.load_time
      #verify the results.
      if returned == false
        if @supervisorweb.SUPfilter_message_page.verifyWarningStartDate == true
           assert(true, "The warning message is display for incorrect start date format.")
           puts "The warning message is display for incorrect start date format."
        elsif @supervisorweb.SUPfilter_message_page.verifyWarningEndDate == true
           assert(true, "The warning message is display for incorrect end date format.")
           puts "The warning message is display for incorrect end date format."
        elsif @supervisorweb.SUPfilter_message_page.getWarningInfo == true
          assert(true, "The warning message is display for incorrect date compare.")
        else
          assert(false, "error: The warning message is not display for incorrect date. ")
        end
      else
        @supervisorweb.SUPheader_page.clickFilters
        save_result = @supervisorweb.SUPfilter_message_page.verifyFilterChange(@input_data2)
        if verifyFilterResult(@input_data2[6]) == true && save_result == true
          assert(true, "The total message number and filter conditions saved as expected")
        elsif verifyFilterResult(@input_data2[6]) == false && save_result == true
          total_message = @supervisorweb.SUPMessageList_page.getMessagesNumber(1)
          assert(false, "error: The expected total message is "+@input_data2[6].to_s+". The actual total message is "+total_message.to_s+".")
        elsif verifyFilterResult(@input_data2[6]) == true && save_result == false
          assert(false, "error: The filter conditions saved failed.")
        else
          assert(false, "error: Both the total message number and filter conditions saved are failed.")
        end         
      end
 
  end 

  #set the conditions in the filter dialog box.
  def configure_message_conditon(i_row)
    @input_data2 = @excel_file2.getUserData(@data_row2,i_row)
    return_vlaue = @supervisorweb.SUPfilter_message_page.setShowCondition(@input_data2)
    if return_vlaue == false
      return false
    end
  end

  #verify the results.
  def verifyFilterResult(expect_total)
    total_message = @supervisorweb.SUPMessageList_page.getMessagesNumber(1)
    if total_message == expect_total.to_i
      return true
    else
      return false
    end
  end
   
end
 

