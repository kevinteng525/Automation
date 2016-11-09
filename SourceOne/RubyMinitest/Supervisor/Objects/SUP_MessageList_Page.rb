require 'watir-webdriver'
require_relative '..\Variables\SUP_Variables_Configure.rb'
require_relative '..\Objects\Supervisor_Objects.rb'
gem "minitest"
require "minitest/autorun"

#Reviewer message list page==============================================================
class SUPMessageListPage < BrowserContainer

  #get the total number of messages;get the selected messages.
  #1 is total number; 3 is selected number.
  def getMessagesNumber(_index)
    if total_selected_text.exists?
      str = total_selected_text.attribute_value("textContent").to_s
      _array = str.split(/ /)
      _number = _array[_index].to_i
      return _number
    else
      puts "Line 20 of SUP_MessageList_Page.rb - error: Total: xxx Selected: xxx cannot be found."
      return false
    end
  end

  #add comment from excel.
  def addComment(excel_data)
    test_data = excel_data[0].to_s.split(",")
    num_row = test_data.size
    if num_row ==1
      before_row = getcommentrow(excel_data[0])
      #single add comment.
      addSinglecomment(excel_data[0], excel_data[1])
      after_row = getcommentrow(excel_data[0])
      if before_row+1 == after_row
        #verify the comment is correct.
        result = verify_comment(excel_data[0], excel_data[1])
        if result == false
          puts "Line 33 of SUP_MessageList_Page.rb - error: the content of the added comment is incorrect."
          return false
        else
          return true
        end
      else
        puts "Line 44 of SUP_MessageList_Page.rb - error: after added a comment, no additional comment be added in view."
        return false  
      end
    else   
      #check selected.
      before_rows = []
      for i in 0..num_row-1
        before_row = getcommentrow(test_data[i].to_i)
        before_rows << before_row
        chekcboxes_chk(test_data[i].to_i-1).click
      end 
      #click on comment selected icon.
      clickCommentSelected
      if excel_data[2] == "no"
        for j in 0..num_row-1
          inputComment(excel_data[1])
          clickOK
        end
      else
        checkApplyRemaining
        inputComment(excel_data[1])
        clickOK
      end
      result = "true"
      #check the comment number added.
      for k in 0..num_row-1
        after_row = getcommentrow(test_data[k].to_i)
        if before_rows[k]+1 == after_row
          result += "true" 
        else
          puts "Line 74 of SUP_MessageList_Page.rb - error: after added a comment, no additional comment be added in view for row #"+test_data[k].to_s
          result += "false"  
        end
      end
      if result.include?"false"
        return false
      else
        return true
      end
    end
  end

  def getSample(num)
    total_message = getMessagesNumber(1)
    range = (2..30).to_a.sample(num)
    return range.sort    
  end

  #comment all.
  def commentAll(excel_data)
    #click on comment all icon.
    clickCommentAll
    sleep(1)
    #click on OK button.
    clickCommentAllOK
    #add comment.
    inputComment(excel_data[1])
    clickOK
    #verify sample comments.
    arrSample = getSample(5)
    p arrSample
   # totalMessage = getMessagesNumber(1)
    #click on first message.
    clickSpecifyMessage(1)
    result = "true"
    for i in 2..30
      for j in 0..arrSample.size-1    
        if arrSample[j].to_i == i
          sleep(15)
          verify_result = verify_comment(i, excel_data[1]) 
          result += verify_result.to_s
          clickSpecifyMessage(i)   
        end  
      end
      @browser.send_keys :down
      sleep(1)
    end
    return result
  end

  #click add comment icon.
  def clickSingleAddComment(irow)
    i_row = irow.to_i
    if single_add_comment_icon(i_row-1).exists?
      single_add_comment_icon(i_row-1).click
      @browser.wait($TIMEOUT)
    else
      puts "Line 131 of SUP_MessageList_Page.rb - error: Add Comment icon cannot be found for row #"+i_row.to_s
    end
  end

  #click view history icon.
  def clickSingleViewHistory(irow)
    i_row = irow.to_i
    if single_view_history_icon(i_row-1).exists?
      single_view_history_icon(i_row-1).click
      @browser.wait($TIMEOUT)
    else
      puts "Line 142 of SUP_MessageList_Page.rb - error: View history icon cannot be found for row #"+i_row.to_s
    end
  end  

  #input comment.
  def inputComment(input_comment)
    if comment_textarea.exists?
      comment_textarea.set(input_comment)
    else
      puts "Line 151 of SUP_MessageList_Page.rb - error: Add Comment text area cannot be found."
    end
  end

  #click on OK button.
  def clickOK
    if ok_button.exists?
      ok_button.click
    else
      puts "Line 160 of SUP_MessageList_Page.rb - error: OK button cannot be found for Add Comment dialog."
    end
  end

  #get the view history content.
  def getHistoryContent
    if view_history_text.exists?
      return view_history_text.value
    else
      puts "Line 169 of SUP_MessageList_Page.rb - error: view history text cannot be found."
    end
  end

  #get the row number in comment textarea.
  def getRowNumber
    text_value = getHistoryContent
    array_text = text_value.split("\n").delete_if{|i| i==""}
    return array_text.size
  end

  #get a row comment.
  def getRow(i_row)   
    arr_comment = getHistoryContent.split("\n").delete_if{|i| i==""}
    total_row = arr_comment.size
    if i_row.to_i > total_row
      puts "Line 185 of SUP_MessageList_Page.rb - error: you input a wrong comment row. it greater than the total rows in this comment area."
    else
      strComment = arr_comment[i_row.to_i-1].to_s
    end
  end

  #get the specified comment.
  def getComment(irow)
    latest_comment = getRow(irow)
    getSubStr(latest_comment, "Comment:")
  end

  #get comment row number.
  def getcommentrow(i_row)
    clickSingleViewHistory(i_row)
    row_no = getRowNumber
    clickOK
    return row_no
  end  

  #add single comment.
  def addSinglecomment(i_row, inputcomment)
    clickSingleAddComment(i_row)
    inputComment(inputcomment)
    clickOK    
  end  

  #verify the latest comment is correct.  
  def verify_comment(row, inputcomment)
    irow = row.to_i
    #click on view history icon.
    clickSingleViewHistory(irow)
    #get the latest history comment.
    str= getComment(1)
    if str == inputcomment
      clickOK
      return true
    else
      clickOK
      puts "Line 224 of SUP_MessageList_Page.rb - Row #"+irow.to_s+" is incorrect."
      return false
    end
  end  

  #click on Comment selected icon.
  def clickCommentSelected
    if comment_selected_button.exists?
      if comment_selected_button.class_name.include?"dijitMenuItemDisabled"
        puts "Line 232 of SUP_MessageList_Page.rb - error: Comment selected icon is disabled. No message is selected."
      else
        comment_selected_button.click
      end
    else
      puts "Line 237 of SUP_MessageList_Page.rb - error: Comment selected icon cannot be found."
    end
  end

  #click on Comment all icon.
  def clickCommentAll
    if apply_to_all_items_button.exists?
      apply_to_all_items_button.click
      sleep(1)
      if comment_all_icon.exists?
        comment_all_icon.click
      else
        puts "Line 249 of SUP_MessageList_Page.rb - error: Comment All icon cannot be found."
      end
    else
      puts "Line 252 of SUP_MessageList_Page.rb - error: Apply to all Items icon cannot be found."
    end
  end
  
  #check specified messages.
  def checkMessages(rows)
    #convert string to array.
    arr_rows= rows.to_s.split(/,/)
    rows_num = arr_rows.size-1
    #check the specified review group.
    for i in 0..rows_num
      if arr_rows[i].to_i == i
        chekcboxes_chk(i).click
      end
    end       
  end

  #click OK button for comment all.
  def clickCommentAllOK
    if commentall_ok_button.exists?
      commentall_ok_button.click
    else
      puts "Line 274 of SUP_MessageList_Page.rb - error: OK button cannot be found on comment all pop up dialog."
    end
  end

  #click on one specified message row header.
  def clickSpecifyMessage(i_row)
    if single_message(i_row.to_i-1).exists?
      single_message(i_row.to_i-1).click
    else
      puts "Line 283 of SUP_MessageList_Page.rb - error: message #"+i_row.to_s+" cannot be found."
    end
  end
  
  #click on one specified message
  def clickSpecifyMessageBody(i_row)
    if single_message_body(i_row.to_i-1).exists?
      single_message_body(i_row.to_i-1).click
    else
      puts "Line 292 of SUP_MessageList_Page.rb - error: message #"+i_row.to_s+" cannot be found."
    end
  end

  #check or uncheck apply remaining check box.
  def checkApplyRemaining
    if apply_remaining_chk.exists?
      apply_remaining_chk.click
    else
      puts "Line 292 of SUP_MessageList_Page.rb - error: the check box of apply remaining items cannot be found."  
    end
  end

#================================== Mark ================================================================================

  #get the mark table rows.
  def getMarkTableRow
    if single_mark_table.exists?
      return single_mark_table.rows.length - 7
    else
      puts "Line 306 of SUP_MessageList_Page.rb - error: single mark table cannot be found."
    end
  end

  #click mark message icon.
  def clickSingleMarkMessage(irow)
    i_row = irow.to_i
    if single_mark_icon(i_row-1).exists?
      single_mark_icon(i_row-1).click
      @browser.wait($TIMEOUT)
    else
      puts "Line 314 of SUP_MessageList_Page.rb - error: Mark message icon cannot be found for row #"+i_row.to_s
    end      
  end

  #select single mark type.
  def clickMarkByType(type, _BPs)
    bp_row = _BPs.to_i
    rows = getMarkTableRow
    num_completely = []
    num_partially = []
    num_escalated = []
    for i in 0..rows-1
      if mark_type_list(i).attribute_value("aria-label").to_s.include?"OK"
        num_completely << i
      elsif mark_type_list(i).attribute_value("aria-label").to_s.include?"Partially"
        num_partially << i
      elsif mark_type_list(i).attribute_value("aria-label").to_s.include?"Escalated"
        num_escalated << i
      end
    end   

    if type == "OK"
     # single_mark_table.[]((num_completely[bp_row].to_i)*rank).click   
      mark_type_list(num_completely[bp_row].to_i).click
      return num_completely.size
    elsif type == "Partially"
     # single_mark_table.[]((num_partially[bp_row].to_i)*rank).click 
      mark_type_list(num_partially[bp_row].to_i).click
      return num_partially.size
    elsif type == "Escalated"
      mark_type_list(num_escalated[bp_row].to_i).click
      return num_escalated.size      
    end  
    
  end  

  #single mark message by type.
  def markMessage(excel_data)
    test_data = excel_data[0].to_s.split(",")
    num_row = test_data.size
    result = "ture"
    for i in 0..num_row-1
      before_comment_rows = getcommentrow(test_data[i].to_i)
      #click single mark icon.
      clickSingleMarkMessage(test_data[i])
      #select type.
      num_type = clickMarkByType(excel_data[2], excel_data[4])
      if num_type ==1 || excel_data[4].to_i != 0
        #input comment.
        inputComment(excel_data[1])
        clickOK
        #verify the row.
        after_comment_rows = getcommentrow(test_data[i].to_i)
        if after_comment_rows == before_comment_rows+excel_data[9].to_i
          result = "true"
        else
          result = "false"
          puts "error: the row count is incorrect with only one BP."
        end
      else
        for j in 0..num_type-2
          #input comment.
          inputComment(excel_data[1])
          clickOK          
        end
        after_comment_rows = getcommentrow(test_data[i].to_i)
        if after_comment_rows == before_comment_rows+excel_data[9].to_i
          result = "true"
        else
          result = "false"   
          puts "error: the row count is incorrect when greater 1 BP."       
        end
      end  

      #verify comment and code.
      mark_result = verifyMark(test_data[i], excel_data[2], excel_data[1], excel_data[5])
      if (mark_result.to_s+result).include?"false"
        return false
      else
        return true
      end
    end    
  end
  
  #get code content.
  def getCode(irow)
    latest_comment = getRow(irow)
    str = getSubStr(latest_comment, "Coded:")
    _index = str.index("Comment:")
    code = str[0,_index].strip
  end  

  def getSubStr(str, token)
    _index = str.index(token)+token.size
    sub_str = str[_index,255].strip
  end

  def verify_code(row, type)
    #click on view history icon.
    clickSingleViewHistory(row)
    #get the latest history code.
    str_code = getCode(1)
    if str_code.include?type
      clickOK
      return true
    else
      clickOK
      return false  
    end
  end
  
  def getColumnStatus(irow)
    i_row = irow.to_i
    if message_column_status(i_row-1).exists?
      status = message_column_status(i_row-1).attribute_value("title")
      return status
    else
      puts "error: row #"+irow.to_s+"'s status column cannot be found."
    end
  end
  
  def verifyMark(row, mark_type, comment, expect_result)
    irow = row.to_i
    result = "true"
    
    #verify the comment.
    comment_result = verify_comment(row, comment)
    result += comment_result.to_s
    #verify the code.
    code_result = verify_code(row, mark_type)
    result += code_result.to_s  
    
    #verify the status in the message list.    
    if getColumnStatus(irow).to_s.include?expect_result
      result += "true"
    else
      puts "error: row #"+irow.to_s+"'s status does not equal to "+expect_result+"."
      result += "false"
    end
    if result.include?"false"
      return false
    else
      return true
    end
  end
  
  #click mark selected.
  def clickMarkSelected
    if mark_selected_icon.exists?
      mark_selected_icon.click
    else
      puts "error: Mark selected icon cannot be found."
    end
  end
  
  #mark selected.
  def markSelectedMessages(excel_data)
    test_data = excel_data[0].to_s.split(",")
    num_row = test_data.size
    result = "true"
    #get the comment rows in the comment dialog box.
    before_rows = []
    for i in 0..num_row-1
      before_rows << getcommentrow(test_data[i].to_i)
      #check selected messages.
      chekcboxes_chk(test_data[i].to_i-1).click
    end     
    #click on mark selected icon.
    clickMarkSelected
    #select type.
    num_type = clickMarkByType(excel_data[2], excel_data[4])
      for j in 0..excel_data[10]-1
        #input comment.
        inputComment(excel_data[1])
        clickOK          
      end
      #get the comment rows in the comment dialog box.
      after_rows = []
      for k in 0..num_row-1
        after_rows << getcommentrow(test_data[k].to_i)
      end
      comment_rows_added = excel_data[9].to_s.split(",")
      for m in 0..num_row-1
        if after_rows[m] == before_rows[m]+comment_rows_added[m].to_i
          result += "true"
        else
          result += "false"   
          puts "error: the row count is incorrect when greater 1 BP."       
        end
        #verify comment and code.
        mark_result = verifyMark(test_data[m], excel_data[2], excel_data[1], excel_data[5])
        if (mark_result.to_s+result).include?"false"
          result += "false"
        else
          result += "true"
        end
      end            
    
    if result.include?"false"
      return false
    else
      return true
    end         
  end
    
  #get the subject text.
  def getSubejctColumnText(irow)
    if message_column_subject(irow.to_i-1).exists?
      return message_column_subject(irow.to_i-1).attribute_value("title").to_s
    else
      puts "error: subeject column cannot be found."
    end
  end
    
  #get the message date.
  def getMessageDateColumnText(irow)
    if message_column_date(irow.to_i-1).exists?
      return message_column_date(irow.to_i-1).attribute_value("title").to_s
    else
      puts "error: message date column cannot be found."
    end
  end  
    
  #get the message date without second.
  def getMessageDateColumnTextWithoutSecond(irow) 
    messagedate = getMessageDateColumnText(irow)
    array_date = messagedate.split(/:/)
    str_am_pm = array_date[2].split(/ /)
    return array_date[0]+":"+array_date[1]+" "+str_am_pm[1]
  end
  
  def SortByColumn(column,order)
    if column_header(column).exists?
      if column_header(column).attribute_value("aria-sort").to_s.include?order
        puts "the order is " + column_header(column).attribute_value("aria-sort").to_s
      else
        column_header(column).click
      end
    else
      puts "Can not find the column header"      
    end
  end  
  private
  
  #Total: xxx Selected: xxx.
  def total_selected_text
    @browser.div(:class => "gridxSummaryMessage")
  end 
  
  #single message rowheader in list.
  def single_message(i_message)
    @browser.div(:id =>"MsgListPane").div(:class => /gridxRow /, :index => i_message)
  end
  
  #single message in list
  def single_message_body(i_message)
    @browser.div(:id =>"MsgListPane").div(:class => /gridxRow /, :index => i_message).td(:class=>"gridxCell")
  end
  
  #single add comment icon.
  def single_add_comment_icon(i_record)
    @browser.div(:id =>"MsgListPane").div(:class => /gridxRow /, :index => i_record).span(:class =>"dijitReset dijitInline dijitIcon toolbar_icon toolbar_icon_addComment")
  end
  
  
  #single view history icon.
  def single_view_history_icon(i_row)
    @browser.div(:id =>"MsgListPane").div(:class => /gridxRow /, :index => i_row).span(:class =>"dijitReset dijitInline dijitIcon toolbar_icon toolbar_icon_viewHistory")
  end 
  
  #single mark message icon.
  def single_mark_icon(i_row)
    @browser.div(:id =>"MsgListPane").div(:class => /gridxRow /, :index => i_row).span(:class =>"dijitReset dijitInline dijitIcon toolbar_icon FlagMessageDropDown")
  end
  
  #Add Comment dialog.
  def comment_textarea
    @browser.textarea(:class =>/dijitTextBox dijitTextArea dijitExpandingTextArea/)
  end
  
  #OK button in add comment dialog or view history dialog.
  def ok_button
    @browser.div(:class =>"dialog-footer").span(:class =>"dijitReset dijitStretch dijitButtonContents")
  end

  #view history text pane.
  def view_history_text
    @browser.div(:class =>"dijitDialog").textarea(:class =>"dijitTextBox dijitTextArea dijitExpandingTextArea dijitTextBoxReadOnly dijitTextAreaReadOnly dijitExpandingTextAreaReadOnly dijitReadOnly")
  end

  #Comment selected icon.
  def comment_selected_button
    @browser.div(:id =>"messageListPane").div(:id =>"dijit_PopupMenuBarItem_1")
  end

  #Apply to all Items icon.
  def apply_to_all_items_button
    @browser.div(:id =>"messageListPane").div(:id =>"dijit_PopupMenuBarItem_2")
  end
  
  #OK button on warning dialog box
  def warning_dialog_OK_bt
    @browser.div(:class =>"buttonRow").span(:id =>"button1")
  end

  #scroll bar.
  def scroll_bar
    @browser.div(:class =>"gridxMain").div(:class =>"gridxVScroller")
  end

  #Check boxes of messages.
  def chekcboxes_chk(i_row)
    @browser.div(:id =>"messageListPane").div(:class =>"gridxMain").div(:class =>"gridxRowHeaderBody").div(:class =>"gridxRowHeaderRow", :index =>i_row)
  end
  
  #OK button on comment all dialog.
  def commentall_ok_button
    @browser.span(:class =>"dijit dijitReset dijitInline dialog-button dijitButton")
  end

  #comment all icon.
  def comment_all_icon
    @browser.body(:class =>"claro").div(:id => "dijit_PopupMenuBarItem_2_dropdown").tbody(:class =>"dijitReset").tr(:id =>/dijit_MenuItem/).td(:id => /dijit_MenuItem/)
  end
  
  #check box of apply to remaining items.
  def apply_remaining_chk
    @browser.input(:class =>"dijitReset dijitCheckBoxInput")
  end

  #Mark drop down list table.
  def single_mark_table
    if @browser.table(:id =>"dijit_Menu_1").exists?
      @browser.table(:id =>"dijit_Menu_1")
    else
      @browser.table(:id =>"dijit_DropDownMenu_0")
    end
  end
  
  #mark types in the mark table.
  def mark_type_list(itype)
    if @browser.table(:id =>"dijit_Menu_1").exists?
      @browser.table(:id =>"dijit_Menu_1").tr(:class =>"dijitReset dijitMenuItem", :index =>itype)
    else
      @browser.table(:id =>"dijit_DropDownMenu_0").tr(:class =>"dijitReset dijitMenuItem", :index =>itype)
    end
  end  
  
  #mark selected icon.
  def mark_selected_icon
    @browser.div(:id =>"messageListPane").div(:id => /dijit_PopupMenuBarItem_/, :index =>0)
  end
  
  #================================columns===============================================================
  #status column.
  def message_column_status(i_row)
    @browser.div(:id =>"MsgListPane").div(:class => /gridxRow /, :index => i_row).table(:class =>"gridxRowTable").td(:class =>"gridxCell    ", :index => 0).img(:title => /This message is/)
  end
    
  #subject column.
  def message_column_subject(i_row)
    @browser.div(:id =>"MsgListPane").div(:class => /gridxRow /, :index => i_row).table(:class =>"gridxRowTable").td(:class =>"gridxCell    ", :index => 4).div(:class =>"gridxCellWidget")
  end
  
  #message date column.
  def message_column_date(i_row)
    @browser.div(:id =>"MsgListPane").div(:class => /gridxRow /, :index => i_row).table(:class =>"gridxRowTable").td(:class =>"gridxCell    ", :index => 5).div(:class =>"gridxCellWidget")
  end
  
  #column header
  def column_header(i_row)
    @browser.div(:id =>"MsgListPane").div(:class =>"gridxHeader").div(:class =>"gridxHeaderRow").div(:class =>"gridxHeaderRowInner").td(:class =>/gridxCell /, :index => i_row)
  end
end 