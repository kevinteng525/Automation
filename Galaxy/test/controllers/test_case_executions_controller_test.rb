require 'test_helper'

class TestCaseExecutionsControllerTest < ActionController::TestCase
  setup do
    @test_case_execution = test_case_executions(:one)
  end

  test "should get index" do
    get :index
    assert_response :success
    assert_not_nil assigns(:test_case_executions)
  end

  test "should get new" do
    get :new
    assert_response :success
  end

  test "should create test_case_execution" do
    assert_difference('TestCaseExecution.count') do
      post :create, test_case_execution: { end_time: @test_case_execution.end_time, job_id: @test_case_execution.job_id, retry_times: @test_case_execution.retry_times, start_time: @test_case_execution.start_time, status: @test_case_execution.status, test_case_id: @test_case_execution.test_case_id }
    end

    assert_redirected_to test_case_execution_path(assigns(:test_case_execution))
  end

  test "should show test_case_execution" do
    get :show, id: @test_case_execution
    assert_response :success
  end

  test "should get edit" do
    get :edit, id: @test_case_execution
    assert_response :success
  end

  test "should update test_case_execution" do
    patch :update, id: @test_case_execution, test_case_execution: { end_time: @test_case_execution.end_time, job_id: @test_case_execution.job_id, retry_times: @test_case_execution.retry_times, start_time: @test_case_execution.start_time, status: @test_case_execution.status, test_case_id: @test_case_execution.test_case_id }
    assert_redirected_to test_case_execution_path(assigns(:test_case_execution))
  end

  test "should destroy test_case_execution" do
    assert_difference('TestCaseExecution.count', -1) do
      delete :destroy, id: @test_case_execution
    end

    assert_redirected_to test_case_executions_path
  end
end
