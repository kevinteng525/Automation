require 'test_helper'

class DiagnosticLogsControllerTest < ActionController::TestCase
  setup do
    @diagnostic_log = diagnostic_logs(:one)
  end

  test "should get index" do
    get :index
    assert_response :success
    assert_not_nil assigns(:diagnostic_logs)
  end

  test "should get new" do
    get :new
    assert_response :success
  end

  test "should create diagnostic_log" do
    assert_difference('DiagnosticLog.count') do
      post :create, diagnostic_log: { component: @diagnostic_log.component, create_time: @diagnostic_log.create_time, log_type: @diagnostic_log.log_type, message: @diagnostic_log.message }
    end

    assert_redirected_to diagnostic_log_path(assigns(:diagnostic_log))
  end

  test "should show diagnostic_log" do
    get :show, id: @diagnostic_log
    assert_response :success
  end

  test "should get edit" do
    get :edit, id: @diagnostic_log
    assert_response :success
  end

  test "should update diagnostic_log" do
    patch :update, id: @diagnostic_log, diagnostic_log: { component: @diagnostic_log.component, create_time: @diagnostic_log.create_time, log_type: @diagnostic_log.log_type, message: @diagnostic_log.message }
    assert_redirected_to diagnostic_log_path(assigns(:diagnostic_log))
  end

  test "should destroy diagnostic_log" do
    assert_difference('DiagnosticLog.count', -1) do
      delete :destroy, id: @diagnostic_log
    end

    assert_redirected_to diagnostic_logs_path
  end
end
